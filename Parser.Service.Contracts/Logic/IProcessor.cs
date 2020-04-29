using System.Collections.Generic;
using System.Threading.Tasks;
using Extractors.Contracts.Types;

namespace Parser.Service.Contracts.Logic {
    public interface IProcessor {
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
        /// Обработка файла по пути
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns></returns>
        Task<Document> ProcessFileByPath(string path);
        
        /// <summary>
        /// Обработка файлов по путям
        /// </summary>
        /// <param name="paths">Список путей к файлам</param>
        /// <returns></returns>
        Task<IEnumerable<Document>> Process(IEnumerable<string> paths);

        /// <summary>
        /// Обработка списка файлов по домену
        /// </summary>
        /// <param name="domain">Домен</param>
        /// <returns></returns>
        Task<IEnumerable<Document>> ProcessFilesByDomain(string domain);
    }
}
