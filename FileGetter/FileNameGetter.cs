using System;
using System.Web;
using MimeKit;

namespace FileGetter {
    public class FileNameGetter {
        public static string Get(string disposition) {
            string result = string.Empty;
            
            if (!string.IsNullOrWhiteSpace(disposition)) {
                try {
                    result = new ContentDisposition(disposition).FileName;
                } catch {
                    var fineNameIndex = disposition.IndexOf("filename", StringComparison.InvariantCultureIgnoreCase);
                    if (fineNameIndex > -1) {
                        var quoteIndex = disposition.IndexOf("''", fineNameIndex, StringComparison.InvariantCultureIgnoreCase);
                        if (quoteIndex > -1) {
                            result = disposition.Substring(quoteIndex + 2, disposition.Length - quoteIndex - 2);
                        }
                    }
                }
            }

            return HttpUtility.UrlDecode(result);
        }
    }
}
