using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Extractors.Contracts.Types;
using JRPC.Service;
using Parser.Service.Contracts.Logic;
using Parser.Service.Contracts.Service;

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
        public async Task<IEnumerable<Document>> ProcessDirectoryByPath(string path, string pattern = "*") {
            var paths = Directory.GetFiles(path, pattern, SearchOption.AllDirectories);
            return await ProcessFilesByPath(paths);
        }

        /// <summary>
        /// Обработка одного файла на диске
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns></returns>
        public async Task<Document> ProcessFileByPath(string path) {
            return await _processor.ProcessFileByPath(path);
        }
        
        /// <summary>
        /// Обработка списка файлов на диске
        /// </summary>
        /// <param name="paths">Путь к файлам</param>
        /// <returns></returns>
        public async Task<IEnumerable<Document>> ProcessFilesByPath(IEnumerable<string> paths) {
            return await _processor.ProcessFilesByPath(paths);
        }
        
        /// <summary>
        /// Обработка одного файла по урлу
        /// </summary>
        /// <param name="url">URL файла</param>
        /// <returns></returns>
        public async Task<Document> ProcessFileByUrl(string url) {
            return await _processor.ProcessFileByUrl(url);
        }
        
        /// <summary>
        /// Обработка списка файлов по урлу
        /// </summary>
        /// <param name="urls">URL'ы файлов</param>
        /// <returns></returns>
        public async Task<IEnumerable<Document>> ProcessFilesByUrl(IEnumerable<string> urls) {
            return await _processor.ProcessFilesByUrl(urls);
        }
        
        /// <summary>
        /// Обработка списка файлов по домену
        /// </summary>
        /// <param name="domain">Домен</param>
        /// <returns></returns>
        public async Task<IEnumerable<Document>> ProcessFilesByDomain(string domain) {
            return await _processor.ProcessFilesByDomain(domain);
        }
    }
}
