using System.Threading.Tasks;

namespace Extractors.Contracts.ContentExtractors {
    /// <summary>
    /// Экстрактор текстов из картинов
    /// </summary>
    public interface IContentImageExtractor {
        /// <summary>
        /// Извлечение текста из картинки
        /// </summary>
        /// <param name="image">Картинка в виде массива байт</param>
        /// <returns></returns>
        Task<string> ExtractTextImage(byte[] image);
    }
}
