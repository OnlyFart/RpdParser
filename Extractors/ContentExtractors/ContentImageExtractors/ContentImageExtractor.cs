using System;
using System.Diagnostics;
using System.IO;

namespace Extractors.ContentExtractors.ContentImageExtractors {
    public class ContentImageExtractor : IContentImageExtractor {
        /// <summary>
        /// Извлечение текста из картинки
        /// </summary>
        /// <param name="image">Картинка в виде массива байт</param>
        /// <returns></returns>
        public string ExtractTextImage(byte[] image) {
            return ParseText("./tesseract", image, "rus");
        }

        private static string ParseText(string tesseractPath, byte[] imageFile, params string[] lang) {
            var result = string.Empty;
            var tempOutputFile = Path.GetTempPath() + Guid.NewGuid();
            var tempImageFile = Path.GetTempFileName();

            try {
                File.WriteAllBytes(tempImageFile, imageFile);

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
                        result = File.ReadAllText(tempOutputFile + ".txt");
                    } else {
                        throw new Exception("Error. Tesseract stopped with an error code = " + process.ExitCode);
                    }
                }
            } finally {
                File.Delete(tempImageFile);
                File.Delete(tempOutputFile + ".txt");
            }

            return result;
        }
    }
}