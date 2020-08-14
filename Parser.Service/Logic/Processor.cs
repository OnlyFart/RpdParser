using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Extractors.Contracts.ContentExtractors;
using Extractors.Contracts.DocumentExtractors;
using Extractors.Contracts.Enums;
using Extractors.Contracts.Types;
using FileGetter;
using Microsoft.Extensions.Configuration;
using NLog;
using Parser.Service.Configs;
using Parser.Service.Contracts.Logic;
using Yandex.Xml;
using Yandex.Xml.Contracts;

namespace Parser.Service.Logic {
    public class Processor : IProcessor {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        
        private readonly List<ExtractorBase> _extractors;
        private readonly List<IDocumentExtractor<DocumentBase>> _documentExtractors;
        private readonly IYandexXmlProvider _yandexXmlProvider;
        private readonly IFileGetter _fileGetter;
        private readonly IConfiguration _config;

        private const string SUCCESS_FOLDER = "Success";
        private const string FAIL_FOLDER = "Fail";

        public Processor(List<ExtractorBase> extractors, List<IDocumentExtractor<DocumentBase>> documentExtractors, IFileGetter fileGetter, IYandexXmlProvider yandexXmlProvider, IConfiguration config) {
            if (config == null) {
                throw new ArgumentNullException(nameof(config));
            }
            
            _extractors = extractors;
            _documentExtractors = documentExtractors;
            _fileGetter = fileGetter;
            _yandexXmlProvider = yandexXmlProvider;
            _config = config;
        }
        
        /// <summary>
        /// Обработка одного файла по урлу
        /// </summary>
        /// <param name="url">URL файла</param>
        /// <returns></returns>
        public async Task<Document> ProcessFileByUrl(string url) {
            var config = _config.GetSection("Processor").Get<ProcessorConfig>();
            
            var file = await _fileGetter.GetFile(url);
            if (file == null) {
                throw new Exception($"Не удалось загрузить файл {url}");
            }

            if (string.IsNullOrWhiteSpace(file.FileName)) {
                throw new Exception($"Не удалось определить имя файла по урлу {url}");
            }
            
            var result = await ProcessFile(file.Bytes, file.FileName);
            result.FileUrl = url;

            string savePath;

            if (result.DocumentContent.DocumentType != DocumentType.Unknown) {
                savePath = await SaveFile(Path.Combine(config.BaseDirectory, SUCCESS_FOLDER, file.Host), file);
            } else {
                savePath = await SaveFile(Path.Combine(config.BaseDirectory, FAIL_FOLDER, file.Host), file);
            }

            result.FilePath = savePath;
            return result;
        }

        /// <summary>
        /// Обработка файла по пути
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns></returns>
        public async Task<Document> ProcessFileByPath(string path) {
            var bytes = await File.ReadAllBytesAsync(path);
            var rpd = await ProcessFile(bytes, path);

            rpd.FilePath = path;

            return rpd;
        }
        
        /// <summary>
        /// Обработка файлов по путям
        /// </summary>
        /// <param name="paths">Список путей к файлам</param>
        /// <returns></returns>
        public async Task<IEnumerable<Document>> ProcessFilesByPath(IEnumerable<string> paths) {
            var config = _config.GetSection("Processor").Get<ProcessorConfig>();
            
            var queue = new ConcurrentQueue<Document>();
            Parallel.ForEach(paths, new ParallelOptions {MaxDegreeOfParallelism = config.MaxParallelThreads}, path => {
                var rpd = ProcessFileByPath(path).Result;
                Log(rpd);
                queue.Enqueue(rpd);
            });
            
            return await Task.FromResult(queue);
        }
        
        /// <summary>
        /// Обработка списка файлов по урлу
        /// </summary>
        /// <param name="urls">URL'ы файлов</param>
        /// <returns></returns>
        public async Task<IEnumerable<Document>> ProcessFilesByUrl(IEnumerable<string> urls) {
            var config = _config.GetSection("Processor").Get<ProcessorConfig>();
            
            var result = new ConcurrentQueue<Document>();
            var processed = 0;
            
            Parallel.ForEach(urls, new ParallelOptions {MaxDegreeOfParallelism = config.MaxParallelThreads}, url => {
                Document rpd;
                Interlocked.Increment(ref processed);
                
                try {
                    rpd = ProcessFileByUrl(url).Result;
                    Log(rpd);
                } catch (Exception ex) {
                    _logger.Error(ex, $"При обработке {url} возникло исключение");
                    
                    rpd = new Document {
                        FileUrl = url,
                        ErrorMessage = ex.Message
                    };
                }
                
                result.Enqueue(rpd);
            });
            
            return await Task.FromResult(result);
        }
        
        /// <summary>
        /// Обработка файла
        /// </summary>
        /// <param name="file">Контент файла в байтах</param>
        /// <param name="extension">Расширения файла</param>
        /// <returns></returns>
        private async Task<Document> ProcessFile(byte[] file, string extension) {
            var result = new Document();

            foreach (var extractor in _extractors.Where(t => t.IsSupport(extension))) {
                var documentContent = extractor.ExtractText(file, extension);
                
                // Сначала попытка определить тип документа на основании текстового содержания
                foreach (var documentExtractor in _documentExtractors) {
                    result.DocumentContent = documentExtractor.Extract(documentContent.Content);
                    if (result.DocumentContent.Success) {
                        return result;
                    }
                }
                
                
                // Если попытка на основе текстового содержания не увенчалась успехом
                // То пытаемся определить по тексту на картинках.
                // Логика разделена, т.к. извлекать текст из картинок довольно долго
                documentContent = await extractor.ExtractImageText(file, extension);
                foreach (var documentExtractor in _documentExtractors) {
                    result.DocumentContent = documentExtractor.Extract(documentContent.Content);
                    result.HasImageContent = documentContent.HasImageContent;
                    if (result.DocumentContent.Success) {
                        return result;
                    }
                }

                return result;
            }

            result.ErrorMessage = "Нет подходящего экстрактора для данного типа файла";
            return result;
        }
        
        /// <summary>
        /// Обработка списка файлов по домену
        /// </summary>
        /// <param name="domain">Домен</param>
        /// <returns></returns>
        public async Task<IEnumerable<Document>> ProcessFilesByDomain(string domain) {
            var config = _config.GetSection("Processor").Get<ProcessorConfig>();
            var urls = new List<string>();
            
            foreach (var pattern in config.XmlPatterns) {
                var xmlResponse = await _yandexXmlProvider.Get(pattern.Replace("{domain}", domain), YandexXmlProvider.MAX_XML_RESULT);
                urls.AddRange(xmlResponse.Items.Select(i => i.Url));
            }
            
            _logger.Info($"Найдено {urls.Count} ссылок для домена {domain}");
            
            return await ProcessFilesByUrl(urls);
        }
        
        /// <summary>
        /// Логирование
        /// </summary>
        /// <param name="document"></param>
        private void Log(Document document) {
            Console.ForegroundColor = document.DocumentContent.DocumentType != DocumentType.Unknown ? (document.HasImageContent ? ConsoleColor.Blue : ConsoleColor.Green) : ConsoleColor.Red;
            Console.OutputEncoding = Encoding.UTF8;
            
            _logger.Info($"Извлечение текста из картинок: [{document.HasImageContent}] "
                         + $"Тип документа: [{document.DocumentContent.DocumentType}] "
                         + $"Информация о результате разбора: {document.DocumentContent.LogMessage()} "
                         + $"Путь к файлу на диске: [{document.FilePath}] "
                         + $"Путь к файлу на сайте: [{document.FileUrl}]");
        }
        
        /// <summary>
        /// Сохранение файла
        /// </summary>
        /// <param name="directory">Директория для сохранения</param>
        /// <param name="file">Файл</param>
        /// <returns></returns>
        private static async Task<string> SaveFile(string directory, FileData file) {
            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }
            
            var savePath = Path.Combine(directory, file.FileName);
            
            // Есть вероятность, что имена файлов будут повторяться
            // Что бы файлы не перетерались, добавлен такой код
            for (var i = 0; File.Exists(savePath); i++) {
                savePath = Path.Combine(directory, $"{i}_{file.FileName}");
            }
                
            await File.WriteAllBytesAsync(savePath, file.Bytes);
            return savePath;
        }
    }
}
