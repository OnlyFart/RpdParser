using System.Collections.Generic;
using System.Threading.Tasks;
using Extractors.Types;
using Extractors.Types.Document;

namespace Parser.Service.Logic {
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

        Task<Document> ProcessFileByPath(string path);
        Task<IEnumerable<Document>> Process(IEnumerable<string> paths);

        /// <summary>
        /// Обработка списка файлов по домену
        /// </summary>
        /// <param name="domain">Домен</param>
        /// <returns></returns>
        Task<IEnumerable<Document>> ProcessFilesByDomain(string domain);
    }
}
