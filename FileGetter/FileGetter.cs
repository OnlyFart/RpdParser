using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;

namespace FileGetter {
    public class FileNetworkGetter : IFileGetter {
        private const int MAX_TRY_COUNT = 3;
        
        public async Task<FileData> GetFile(string path) {
            var uri = new Uri(path);

            for (var i = 0; i < MAX_TRY_COUNT; i++) {
                var request = (HttpWebRequest) WebRequest.Create(uri);
                request.Headers["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
                request.Headers["Accept-Encoding"] = "gzip, deflate, br";
                request.Headers["Accept-Language"] = "en-US,en;q=0.9";
                request.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.122 Safari/537.36";

                var result = new FileData();
                try {
                    using (var response = (HttpWebResponse) (await request.GetResponseAsync())) {
                        var disposition = response.Headers["Content-Disposition"];
                        if (!string.IsNullOrWhiteSpace(disposition)) {
                            result.FileName = HttpUtility.UrlDecode(new ContentDisposition(disposition).FileName);
                        } else {
                            result.FileName = HttpUtility.UrlDecode(uri.Segments.Last());
                        }

                        using (var stream = response.GetResponseStream()) {
                            if (stream != null) {
                                using (var ms = new MemoryStream()) {
                                    await stream.CopyToAsync(ms);
                                    result.Bytes = ms.ToArray();
                                    result.Host = uri.Host;
                                }
                            }
                        }
                    }

                    if (result.Bytes == null || result.Bytes.Length == 0) {
                        continue;
                    }
                    
                    return result;
                } catch { }
            }

            return null;
        }
    }
}
