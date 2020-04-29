using Extractors.Types.Document;

namespace Extractors.DocumentExtractors {
    public interface IDocumentExtractor<out T> where T : DocumentBase {
        T Extract(string content);
    }
}
