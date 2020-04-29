using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Extractors.ContentExtractors;
using Extractors.DocumentExtractors;
using Extractors.Types.Document;
using Extractors.Types.Enums;
using FileGetter;
using Parser.Service.Configs;
using Yandex.Xml.Contracts;

namespace Parser.Service.Logic {
    public class Processor : IProcessor {
        private readonly List<ExtractorBase> _extractors;
        private readonly List<IDocumentExtractor<DocumentBase>> _documentExtractors;
        private readonly IFileGetter _fileGetter;
        private readonly IYandexXmlProvider _yandexXmlProvider;
        private readonly ProcessorConfig _config;

        private const string ALL_FOLDER = "All";
        private const string SUCCESS_FOLDER = "Success";
        private const string FAIL_FOLDER = "Fail";

        public Processor(List<ExtractorBase> extractors, List<IDocumentExtractor<DocumentBase>> documentExtractors, IFileGetter fileGetter, IYandexXmlProvider yandexXmlProvider, ProcessorConfig config) {
            if (config == null) {
                throw new ArgumentNullException(nameof(config));
            }
            
            if (config.MaxParallelThreads <= 0) {
                throw new ArgumentOutOfRangeException(nameof(config.MaxParallelThreads));
            }

            if (string.IsNullOrWhiteSpace(config.BaseDirectory)) {
                throw new ArgumentNullException(nameof(config.BaseDirectory));
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
            var file = await _fileGetter.GetFile(url);
            if (file == null) {
                throw new Exception($"Не удалось загрузить файл {url}");
            }

            if (string.IsNullOrWhiteSpace(file.FileName)) {
                throw new Exception($"Не удалось определить имя файла по урлу {url}");
            }
            
            var result = await ProcessFile(file.Bytes, file.FileName);
            result.FileUrl = url;

            // Есть вероятность, что имена файлов будут повторяться
            // Что бы файлы не перетерались, добавлен такой код
            var savePath = await SaveFile(Path.Combine(_config.BaseDirectory, ALL_FOLDER, file.Host), file);

            if (result.DocumentContent.DocumentType != DocumentType.Unknown) {
                await SaveFile(Path.Combine(_config.BaseDirectory, SUCCESS_FOLDER, result.DocumentContent.DocumentType.ToString(), file.Host), file);
            } else {
                await SaveFile(Path.Combine(_config.BaseDirectory, FAIL_FOLDER, file.Host), file);
            }

            result.FilePath = savePath;
            return result;
        }

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
        
        /// <summary>
        /// Обработка списка файлов по урлу
        /// </summary>
        /// <param name="urls">URL'ы файлов</param>
        /// <returns></returns>
        public async Task<IEnumerable<Document>> ProcessFilesByUrl(IEnumerable<string> urls) {
            var result = new ConcurrentQueue<Document>();
            var sw = Stopwatch.StartNew();
            int processed = 0;

            Parallel.ForEach(urls, new ParallelOptions {MaxDegreeOfParallelism = _config.MaxParallelThreads}, url => {
                Document rpd;
                try {
                    rpd = ProcessFileByUrl(url).Result;
                    var seconds = sw.ElapsedMilliseconds * 1.0m / 1000;
                    var speed = processed / seconds;
                    Console.ForegroundColor = rpd.DocumentContent.DocumentType != DocumentType.Unknown ? (rpd.HasImageContent ? ConsoleColor.Blue : ConsoleColor.Green) : ConsoleColor.Red;
                    Console.WriteLine($"{processed} {(int)(seconds / 60)} {(int)speed} {rpd.HasImageContent} {rpd.FilePath} {rpd.DocumentContent.DocumentType}");
                } catch (Exception ex) {
                    rpd = new Document {
                        FileUrl = url,
                        ErrorMessage = ex.Message
                    };
                }
                
                Interlocked.Increment(ref processed);
                result.Enqueue(rpd);
            });
            
            return await Task.FromResult(result);
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
        public async Task<IEnumerable<Document>> Process(IEnumerable<string> paths) {
            var queue = new ConcurrentQueue<Document>();
            var sw = Stopwatch.StartNew();
            var processed = 0;

            Parallel.ForEach(paths, new ParallelOptions {MaxDegreeOfParallelism = _config.MaxParallelThreads}, path => {
                var rpd = ProcessFileByPath(path).Result;
                Interlocked.Increment(ref processed);
                var seconds = sw.ElapsedMilliseconds * 1.0m / 1000;
                var speed = processed / seconds;
                Console.ForegroundColor = rpd.DocumentContent.DocumentType != DocumentType.Unknown ? (rpd.HasImageContent ? ConsoleColor.Blue : ConsoleColor.Green) : ConsoleColor.Red;
                Console.WriteLine($"{processed} {(int)(seconds / 60)} {(int)speed} {rpd.HasImageContent} {rpd.FilePath} {rpd.DocumentContent.DocumentType}");
                queue.Enqueue(rpd);
            });

            Console.WriteLine(sw.ElapsedMilliseconds / 1000 / 60);
            return await Task.FromResult(queue);
        }

        private async Task<Document> ProcessFile(byte[] file, string extension) {
            var result = new Document();

            foreach (var extractor in _extractors.Where(t => t.IsSupport(extension))) {
                var documentContent = extractor.ExtractText(file, extension);
                
                // Сначала попытка определить тип документа на основании текстового содержания
                foreach (var documentExtractor in _documentExtractors) {
                    result.DocumentContent = documentExtractor.Extract(documentContent.Content);
                    if (result.DocumentContent.DocumentType != DocumentType.Unknown) {
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
                    if (result.DocumentContent.DocumentType != DocumentType.Unknown) {
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
            var urls = new List<string>();
            
            foreach (var pattern in _config.XmlPatterns) {
                var xmlResponse = await _yandexXmlProvider.Get(pattern.Replace("{domain}", domain), 500);
                urls.AddRange(xmlResponse.Items.Select(i => i.Url));
            }
            
            return await ProcessFilesByUrl(urls);
        }
    }
}
