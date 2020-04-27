using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Extractors.Types;
using JRPC.Service;
using Parser.Service.Contracts;
using Parser.Service.Logic;

namespace Parser.Service.Service {
    public class ParserService : JRpcModule, IParserService {
        private readonly IProcessor _processor;

        public ParserService(IProcessor processor) {
            _processor = processor;
        }
        
        /// <summary>
        /// Обработка всех файлов директории на диске
        /// </summary>
        /// <param name="path">Путь в директории</param>
        /// <param name="pattern">Шаблон для поиска файлов. По умолчанию *</param>
        /// <returns></returns>
        public async Task<IEnumerable<Rpd>> ProcessDirectoryByPath(string path, string pattern = "*") {
            var paths = Directory.GetFiles(path, pattern, SearchOption.AllDirectories);
            return await ProcessFilesByPath(paths);
        }

        /// <summary>
        /// Обработка одного файла на диске
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns></returns>
        public async Task<Rpd> ProcessFileByPath(string path) {
            return await _processor.ProcessFileByPath(path);
        }
        
        /// <summary>
        /// Обработка списка файлов на диске
        /// </summary>
        /// <param name="paths">Путь к файлам</param>
        /// <returns></returns>
        public async Task<IEnumerable<Rpd>> ProcessFilesByPath(IEnumerable<string> paths) {
            return await _processor.Process(paths);
        }
        
        /// <summary>
        /// Обработка одного файла по урлу
        /// </summary>
        /// <param name="url">URL файла</param>
        /// <returns></returns>
        public async Task<Rpd> ProcessFileByUrl(string url) {
            return await _processor.ProcessFileByUrl(url);
        }
        
        /// <summary>
        /// Обработка списка файлов по урлу
        /// </summary>
        /// <param name="urls">URL'ы файлов</param>
        /// <returns></returns>
        public async Task<IEnumerable<Rpd>> ProcessFilesByUrl(IEnumerable<string> urls) {
            return await _processor.ProcessFilesByUrl(urls);
        }
        
        /// <summary>
        /// Обработка списка файлов по домену
        /// </summary>
        /// <param name="domain">Домен</param>
        /// <returns></returns>
        public async Task<IEnumerable<Rpd>> ProcessFilesByDomain(string domain) {
            return await _processor.ProcessFilesByDomain(domain);
        }
    }
}
