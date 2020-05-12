using System;
using System.Net.Mime;
using System.Web;
using NLog;

namespace FileGetter {
    public class FileNameGetter {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        
        public static string Get(string disposition) {
            string result = string.Empty;
            
            if (!string.IsNullOrWhiteSpace(disposition)) {
                try {
                    result = new ContentDisposition(disposition).FileName;
                } catch {
                    var fineNameIndex = disposition.IndexOf("filename", StringComparison.InvariantCultureIgnoreCase);
                    if (fineNameIndex > -1) {
                        var quotesIndex = disposition.IndexOf("''", fineNameIndex, StringComparison.InvariantCultureIgnoreCase);
                        if (quotesIndex > -1) {
                            result = disposition.Substring(quotesIndex + 2, disposition.Length - quotesIndex - 2);
                        } else {
                            quotesIndex = disposition.IndexOf("\"", fineNameIndex, StringComparison.InvariantCultureIgnoreCase);
                            if (quotesIndex > -1) {
                                result = disposition.Substring(quotesIndex + 1, disposition.Length - quotesIndex - 2);
                            } else {
                                _logger.Warn($"Не удалось получить имя файла из {disposition}");
                            }
                        }
                    }
                }
            }

            return HttpUtility.UrlDecode(result);
        }
    }
}
