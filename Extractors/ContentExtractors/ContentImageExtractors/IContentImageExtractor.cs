using System.Threading.Tasks;

namespace Extractors.ContentExtractors.ContentImageExtractors {
    public interface IContentImageExtractor {
        Task<string> ExtractTextImage(byte[] image);
    }
}
