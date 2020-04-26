using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Extractors.ContentExtractors;
using Extractors.DataExtractors;
using Extractors.Types;
using FileGetter;
using Yandex.Xml.Contracts;

namespace Parser.Service.Logic {
    public class Processor {
        private readonly List<ExtractorBase> _extractors;
        private readonly RpdContentExtractor _rpdContentExtractor;
        private readonly IFileGetter _fileGetter;
        private readonly IYandexXml _yandexXml;

        public Processor(List<ExtractorBase> extractors, RpdContentExtractor rpdContentExtractor, IFileGetter fileGetter, IYandexXml yandexXml) {
            _extractors = extractors;
            _rpdContentExtractor = rpdContentExtractor;
            _fileGetter = fileGetter;
            _yandexXml = yandexXml;
        }
        
        /// <summary>
        /// Обработка одного файла по урлу
        /// </summary>
        /// <param name="url">URL файла</param>
        /// <returns></returns>
        public async Task<Rpd> ProcessFileByUrl(string url) {
            var file = await _fileGetter.GetFile(url);
            if (file == null) {
                throw new Exception($"Не смогли загрузить файл {url}");
            }

            if (string.IsNullOrWhiteSpace(file.FileName)) {
                throw new Exception($"Не смогли определить имя файла по урлу {url}");
            }

            await File.WriteAllBytesAsync(file.FileName, file.Bytes);
            
            var result = await ProcessFile(file.Bytes, file.FileName);
            result.FileUrl = url;
            return result;
        }
        
        /// <summary>
        /// Обработка списка файлов по урлу
        /// </summary>
        /// <param name="urls">URL'ы файлов</param>
        /// <returns></returns>
        public async Task<IEnumerable<Rpd>> ProcessFilesByUrl(IEnumerable<string> urls) {
            var result = new ConcurrentQueue<Rpd>();

            var processFileBlock = new ActionBlock<string>(
                async url => {
                    Rpd rpd;
                    try {
                        rpd = await ProcessFileByUrl(url);
                    } catch (Exception ex) {
                        rpd = new Rpd {
                            FileUrl = url,
                            ErrorMessage = ex.Message
                        };
                    }

                    result.Enqueue(rpd);
                }, new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = 20
                });
            
            foreach (var path in urls) {
                await processFileBlock.SendAsync(path);
            }
            
            processFileBlock.Complete();
            await processFileBlock.Completion;

            return result;
        }

        public async Task<Rpd> ProcessFile(string path) {
            var bytes = await File.ReadAllBytesAsync(path);
            var rpd = await ProcessFile(bytes, path);

            rpd.FilePath = path;

            return rpd;
        }

        public async Task<IEnumerable<Rpd>> Process(IEnumerable<string> paths) {
            var queue = new ConcurrentQueue<Rpd>();
            var sw = Stopwatch.StartNew();
            var processed = 0;
            
            var processFileBlock = new ActionBlock<string>(
                async path => {
                    var rpd = await ProcessFile(path);
                    Interlocked.Increment(ref processed);
                    var seconds = sw.ElapsedMilliseconds * 1.0m / 1000;
                    var speed = processed / seconds;
                    Console.ForegroundColor = rpd.RpdContent.IsRpd ? (rpd.HasImageContent ? ConsoleColor.Blue : ConsoleColor.Green) : ConsoleColor.Red;
                    Console.WriteLine($"{processed} {(int)(seconds / 60)} {(int)speed} {rpd.HasImageContent} {rpd.FilePath} {string.Join(", ", rpd.RpdContent.Codes.Distinct())}");
                    queue.Enqueue(rpd);
                }, new ExecutionDataflowBlockOptions
                {
                    MaxDegreeOfParallelism = 20
                });
            
            foreach (var path in paths) {
                await processFileBlock.SendAsync(path);
            }
            
            processFileBlock.Complete();
            await processFileBlock.Completion;

            Console.WriteLine(sw.ElapsedMilliseconds / 1000 / 60);
            return queue;
        }

        private async Task<Rpd> ProcessFile(byte[] file, string extension) {
            var result = new Rpd();

            foreach (var extractor in _extractors.Where(t => t.IsSupport(extension))) {
                var extract = extractor.ExtractText(file, extension);
                
                result.RpdContent = _rpdContentExtractor.Extract(extract.Content);
                if (result.RpdContent.IsRpd) {
                    return result;
                }
                
                extract = await extractor.ExtractImageText(file, extension);
                    
                result.RpdContent = _rpdContentExtractor.Extract(extract.Content);
                result.HasImageContent = extract.HasImageContent;

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
        public async Task<IEnumerable<Rpd>> ProcessFilesByDomain(string domain) {
            var xmlResponse = await _yandexXml.Get($"site:{domain} mime:pdf \"Рабочая программа\"", 500);
            return await ProcessFilesByUrl(xmlResponse.Items.Select(i => i.Url));
        }
    }
}
