using Extractors.Contracts.Types;

namespace Extractors.Contracts.DocumentExtractors {
    /// <summary>
    /// Экстрактор типа файла из текста извлеченного из документа
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDocumentExtractor<out T> where T : DocumentBase {
        T Extract(string content);
    }
}
