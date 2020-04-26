using System.Threading.Tasks;

namespace FileGetter {
    /// <summary>
    /// Получатеть контентов из интернета
    /// </summary>
    public interface IFileGetter {
        Task<FileData> GetFile(string path);
    }
}
