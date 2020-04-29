using System.Threading.Tasks;
using Extractors.Contracts.Types;

namespace Extractors.Contracts.ContentExtractors {
    public abstract class ExtractorBase {
        protected readonly IContentImageExtractor _imageExtractor;

        protected ExtractorBase (IContentImageExtractor imageExtractor) {
            _imageExtractor = imageExtractor;
        }
        
        /// <summary>
        /// Проверка на поддержку файла данным экстрактором
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns></returns>
        public abstract bool IsSupport(string path);

        /// <summary>
        /// Извлечение текста из документа
        /// </summary>
        /// <param name="bytes">Файл</param>
        /// <param name="extension">Расширение файла</param>
        /// <returns></returns>
        public abstract DocumentContent ExtractText(byte[] bytes, string extension);
        
        /// <summary>
        /// Извлечение текста из картинок в документе
        /// </summary>
        /// <param name="bytes">Файл</param>
        /// <param name="extension">Расширение файла</param>
        /// <returns></returns>
        public abstract Task<DocumentContent> ExtractImageText(byte[] bytes, string extension);
    }
}