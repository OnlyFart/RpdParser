using System;
using System.Linq;
using System.Net.Mime;
using System.Web;
using NLog;

namespace FileGetter {
    public static class FileNameGetter {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        
        public static string Get(string disposition) {
            if (string.IsNullOrWhiteSpace(disposition)) {
                return string.Empty;
            }
            
            var result = string.Empty;
            
            try {
                result = new ContentDisposition(disposition).FileName;
            } catch {
                foreach (var item in disposition.Split(";").Select(i => i.Trim())) {
                    const string FILENAME = "filename";
                    var fileNameIndex = item.IndexOf(FILENAME, StringComparison.InvariantCultureIgnoreCase);
                    if (fileNameIndex > -1) {
                        var quotesIndex = item.IndexOf("''", fileNameIndex, StringComparison.InvariantCultureIgnoreCase);
                        if (quotesIndex > -1) {
                            result = item.Substring(quotesIndex + 2, item.Length - quotesIndex - 2);
                        } else {
                            quotesIndex = item.IndexOf("\"", fileNameIndex, StringComparison.InvariantCultureIgnoreCase);
                            if (quotesIndex > -1) {
                                result = item.Substring(quotesIndex + 1, item.Length - quotesIndex - 2);
                            } else {
                                result = item.Substring(fileNameIndex + FILENAME.Length + 1, item.Length - FILENAME.Length - 1);
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(result)) {
                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(result)) {
                _logger.Warn($"Не удалось получить имя файла из {disposition}");
            }

            return HttpUtility.UrlDecode(result);
        }
    }
}
