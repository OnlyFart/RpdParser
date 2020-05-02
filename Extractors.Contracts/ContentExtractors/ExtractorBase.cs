using System.Text.RegularExpressions;
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
        protected abstract DocumentContent ExtractTextInternal(byte[] bytes, string extension);
        
        /// <summary>
        /// Извлечение текста из картинок в документе
        /// </summary>
        /// <param name="bytes">Файл</param>
        /// <param name="extension">Расширение файла</param>
        /// <returns></returns>
        protected abstract Task<DocumentContent> ExtractImageTextInternal(byte[] bytes, string extension);

        /// <summary>
        /// Извлечение текста из документа
        /// </summary>
        /// <param name="bytes">Файл</param>
        /// <param name="extension">Расширение файла</param>
        /// <returns></returns>
        public DocumentContent ExtractText(byte[] bytes, string extension) {
            return FormatText(ExtractTextInternal(bytes, extension));
        }

        /// <summary>
        /// Извлечение текста из картинок в документе
        /// </summary>
        /// <param name="bytes">Файл</param>
        /// <param name="extension">Расширение файла</param>
        /// <returns></returns>
        public async Task<DocumentContent> ExtractImageText(byte[] bytes, string extension) {
            return FormatText(await ExtractImageTextInternal(bytes, extension));
        }
        
        /// <summary>
        /// Постобработка извлеченного текста
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static DocumentContent FormatText(DocumentContent content) {
            if (content == null || string.IsNullOrWhiteSpace(content.Content)) {
                return content;
            }
            
            content.Content = Regex.Replace(content.Content, "\\r|\\n|\\s", string.Empty);
            return content;
        }
    }
}