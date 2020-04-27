using System.Collections.Generic;
using System.Threading.Tasks;
using Extractors.Types;

namespace Parser.Service.Logic {
    public interface IProcessor {
        /// <summary>
        /// Обработка одного файла по урлу
        /// </summary>
        /// <param name="url">URL файла</param>
        /// <returns></returns>
        Task<Rpd> ProcessFileByUrl(string url);

        /// <summary>
        /// Обработка списка файлов по урлу
        /// </summary>
        /// <param name="urls">URL'ы файлов</param>
        /// <returns></returns>
        Task<IEnumerable<Rpd>> ProcessFilesByUrl(IEnumerable<string> urls);

        Task<Rpd> ProcessFileByPath(string path);
        Task<IEnumerable<Rpd>> Process(IEnumerable<string> paths);

        /// <summary>
        /// Обработка списка файлов по домену
        /// </summary>
        /// <param name="domain">Домен</param>
        /// <returns></returns>
        Task<IEnumerable<Rpd>> ProcessFilesByDomain(string domain);
    }
}
