using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Extractors.ContentExtractors.ContentImageExtractors {
    /// <summary>
    /// Экстрактор текстов из картинов
    /// </summary>
    public class ContentImageExtractor : IContentImageExtractor {
        /// <summary>
        /// Извлечение текста из картинки
        /// </summary>
        /// <param name="image">Картинка в виде массива байт</param>
        /// <returns></returns>
        public async Task<string> ExtractTextImage(byte[] image) {
            return await ParseText("./tesseract", image, "rus");
        }
        
        /// <summary>
        /// Извлечение текста из картинки
        /// </summary>
        /// <param name="tesseractPath">Пусь к папке с tesseract</param>
        /// <param name="image">Картинка в виде массива байт</param>
        /// <param name="lang">Языки</param>
        /// <returns></returns>
        private static async Task<string> ParseText(string tesseractPath, byte[] image, params string[] lang) {
            var result = string.Empty;
            var tempOutputFile = Path.GetTempPath() + Guid.NewGuid();
            var tempImageFile = Path.GetTempFileName();

            try {
                await File.WriteAllBytesAsync(tempImageFile, image);

                var info = new ProcessStartInfo {
                    WorkingDirectory = tesseractPath,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    FileName = "cmd.exe",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    Arguments = "/c tesseract.exe " +
                                tempImageFile + " " +
                                tempOutputFile +
                                " -l " + string.Join("+", lang)
                };

                using (var process = Process.Start(info)) {
                    process.WaitForExit();
                    if (process.ExitCode == 0) {
                        result = await File.ReadAllTextAsync(tempOutputFile + ".txt");
                    } else {
                        throw new Exception("Error. Tesseract stopped with an error code = " + process.ExitCode);
                    }
                }
            } catch {
                
            } finally {
                File.Delete(tempImageFile);
                File.Delete(tempOutputFile + ".txt");
            }

            return result;
        }
    }
}