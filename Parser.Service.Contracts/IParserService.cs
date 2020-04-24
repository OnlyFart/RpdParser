using System.Collections.Generic;
using Extractors.Types;

namespace Parser.Service.Contracts {
    public interface IParserService {
        /// <summary>
        /// Обработка всех файлов директории на диске
        /// </summary>
        /// <param name="path">Путь в директории</param>
        /// <param name="pattern">Шаблон для поиска файлов. По умолчанию *</param>
        /// <returns></returns>
        IEnumerable<Rpd> ProcessDirectoryByPath(string path, string pattern = "*");

        /// <summary>
        /// Обработка одного файла на диске
        /// </summary>
        /// <param name="path">Путь к файлу на диске</param>
        /// <returns></returns>
        Rpd ProcessFileByPath(string path);

        /// <summary>
        /// Обработка списка файлов на диске
        /// </summary>
        /// <param name="paths">Путь к файлам</param>
        /// <returns></returns>
        IEnumerable<Rpd> ProcessFilesByPath(IEnumerable<string> paths);

        /// <summary>
        /// Обработка одного файла по урлу
        /// </summary>
        /// <param name="url">URL файла</param>
        /// <returns></returns>
        Rpd ProcessFileByUrl(string url);

        /// <summary>
        /// Обработка списка файлов по урлу
        /// </summary>
        /// <param name="urls">URL'ы файлов</param>
        /// <returns></returns>
        IEnumerable<Rpd> ProcessFilesByUrl(IEnumerable<string> urls);

        /// <summary>
        /// Обработка одного файла
        /// </summary>
        /// <param name="file">Файл</param>
        /// <returns></returns>
        Rpd ProcessFile(RpdFile file);
        
        /// <summary>
        /// Обработка одного файлов
        /// </summary>
        /// <param name="files">Файл</param>
        /// <returns></returns>
        IEnumerable<Rpd> ProcessFiles(IEnumerable<RpdFile> files);
    }
}
