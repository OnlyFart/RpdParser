using System.Collections.Generic;
using System.Threading.Tasks;
using Extractors.Types;
using Extractors.Types.Document;

namespace Parser.Service.Contracts {
    public interface IParserService {
        /// <summary>
        /// Обработка всех файлов директории на диске
        /// </summary>
        /// <param name="path">Путь в директории</param>
        /// <param name="pattern">Шаблон для поиска файлов. По умолчанию *</param>
        /// <returns></returns>
        Task<IEnumerable<Document>> ProcessDirectoryByPath(string path, string pattern = "*");

        /// <summary>
        /// Обработка одного файла на диске
        /// </summary>
        /// <param name="path">Путь к файлу на диске</param>
        /// <returns></returns>
        Task<Document> ProcessFileByPath(string path);

        /// <summary>
        /// Обработка списка файлов на диске
        /// </summary>
        /// <param name="paths">Путь к файлам</param>
        /// <returns></returns>
        Task<IEnumerable<Document>> ProcessFilesByPath(IEnumerable<string> paths);

        /// <summary>
        /// Обработка одного файла по урлу
        /// </summary>
        /// <param name="url">URL файла</param>
        /// <returns></returns>
        Task<Document> ProcessFileByUrl(string url);

        /// <summary>
        /// Обработка списка файлов по урлу
        /// </summary>
        /// <param name="urls">URL'ы файлов</param>
        /// <returns></returns>
        Task<IEnumerable<Document>> ProcessFilesByUrl(IEnumerable<string> urls);
        
        /// <summary>
        /// Обработка списка файлов по домену
        /// </summary>
        /// <param name="domain">Домен</param>
        /// <returns></returns>
        Task<IEnumerable<Document>> ProcessFilesByDomain(string domain);
    }
}
