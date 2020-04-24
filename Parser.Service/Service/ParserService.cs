using System.Collections.Generic;
using System.IO;
using System.Linq;
using Extractors.Types;
using JRPC.Service;
using Parser.Service.Contracts;
using Parser.Service.Logic;

namespace Parser.Service.Service {
    public class ParserService : JRpcModule, IParserService {
        private readonly Processor _processor;

        public ParserService(Processor processor) {
            _processor = processor;
        }
        
        /// <summary>
        /// Обработка всех файлов директории на диске
        /// </summary>
        /// <param name="path">Путь в директории</param>
        /// <param name="pattern">Шаблон для поиска файлов. По умолчанию *</param>
        /// <returns></returns>
        public IEnumerable<Rpd> ProcessDirectoryByPath(string path, string pattern = "*") {
            var paths = Directory.GetFiles(path, pattern, SearchOption.AllDirectories).ToList();
            return ProcessFilesByPath(paths);
        }

        /// <summary>
        /// Обработка одного файла
        /// </summary>
        /// <param name="file">Файл</param>
        /// <returns></returns>
        public Rpd ProcessFile(RpdFile file) {
            return _processor.ProcessFile(file);
        }
        
        /// <summary>
        /// Обработка файлов
        /// </summary>
        /// <param name="files">Файлы</param>
        /// <returns></returns>
        public IEnumerable<Rpd> ProcessFiles(IEnumerable<RpdFile> files) {
            return _processor.Process(files);
        }
        
        /// <summary>
        /// Обработка одного файла на диске
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns></returns>
        public Rpd ProcessFileByPath(string path) {
            return _processor.ProcessFile(path);
        }
        
        /// <summary>
        /// Обработка списка файлов на диске
        /// </summary>
        /// <param name="paths">Путь к файлам</param>
        /// <returns></returns>
        public IEnumerable<Rpd> ProcessFilesByPath(IEnumerable<string> paths) {
            return _processor.Process(paths);
        }
        
        /// <summary>
        /// Обработка одного файла по урлу
        /// </summary>
        /// <param name="url">URL файла</param>
        /// <returns></returns>
        public Rpd ProcessFileByUrl(string url) {
            throw new System.NotImplementedException();
        }
        
        /// <summary>
        /// Обработка списка файлов по урлу
        /// </summary>
        /// <param name="urls">URL'ы файлов</param>
        /// <returns></returns>
        public IEnumerable<Rpd> ProcessFilesByUrl(IEnumerable<string> urls) {
            throw new System.NotImplementedException();
        }
    }
}
