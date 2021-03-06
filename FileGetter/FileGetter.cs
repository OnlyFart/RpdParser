using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using FileGetter.Configs;
using Microsoft.Extensions.Configuration;
using NLog;

namespace FileGetter {
    /// <summary>
    /// Получатор файлов из интернета
    /// </summary>
    public class FileNetworkGetter : IFileGetter {
        private readonly IConfiguration _config;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        
        public FileNetworkGetter(IConfiguration config) {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }
        
        /// <summary>
        /// Получение файла по его адресу
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public async Task<FileData> GetFile(string address) {
            var uri = new Uri(address);

            var config = _config.GetSection("FileGetter").Get<FileGetterConfig>();
            for (var i = 0; i < config.MaxTryCount; i++) {
                var request = (HttpWebRequest) WebRequest.Create(uri);
                request.Headers["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
                request.Headers["Accept-Encoding"] = "gzip, deflate, br";
                request.Headers["Accept-Language"] = "en-US,en;q=0.9";
                request.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.122 Safari/537.36";
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
                
                var result = new FileData();
                try {
                    using (var response = (HttpWebResponse) (await request.GetResponseAsync())) {
                        result.FileName = FileNameGetter.Get(response.Headers["Content-Disposition"]);
                        if (string.IsNullOrEmpty(result.FileName)) {
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
                } catch (WebException ex) {
                    if (ex.Response is HttpWebResponse errorResponse && errorResponse.StatusCode == HttpStatusCode.NotFound) {
                        throw new Exception($"По адресу {address} получен 404 статус");
                    }
                    
                    _logger.Error(ex, $"При обработке {address} возникло исключение");
                    await Task.Delay(config.ErrorDelayMs);
                } catch(Exception ex) {
                    _logger.Error(ex, $"При обработке {address} возникло исключение");
                    await Task.Delay(config.ErrorDelayMs);
                }
            }

            return null;
        }
    }
}
