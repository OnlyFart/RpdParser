using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using NLog;
using Yandex.Xml.Configs;
using Yandex.Xml.Contracts;
using Yandex.Xml.Contracts.Types;

namespace Yandex.Xml {
    /// <summary>
    /// Провайдер к YandexXml
    /// </summary>
    public class YandexXmlProvider : IYandexXmlProvider {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        
        private readonly IConfiguration _config;
        private const string REQUEST_PATTERN = "https://yandex.com/search/xml?user={0}&key={1}&query={2}&l10n=en&sortby=rlv&filter=none&groupby=attr%3D%22%22.mode%3Dflat.groups-on-page%3D{3}.docs-in-group%3D1&page={4}";
        public const int MAX_XML_RESULT = 250;

        public YandexXmlProvider(IConfiguration config) {
            if (config == null) {
                throw new ArgumentNullException(nameof(config));
            }

            _config = config;
        }
        
        /// <summary>
        /// Получение данных из Yandex.Xml
        /// </summary>
        /// <param name="query">Запрос</param>
        /// <param name="needResult">Нужное количество результатов (максимум 250. Ограничений яндекса)</param>
        /// <returns></returns>
        public async Task<YandexXmlResponse> Get(string query, int needResult) {
            var result = new YandexXmlResponse();
            var page = 0;
            var docOnPage = 100;
            
            var config = _config.GetSection("FileGetter").Get<YandexXmlConfig>();
            do {
                var url = string.Format(REQUEST_PATTERN, config.User, config.Key, HttpUtility.UrlEncode(query), docOnPage, page++);
                var response = await GetStringContent(url);
                if (response == null) {
                    return result;
                }

                var xmlResponse = Parse(response);
                if (!string.IsNullOrWhiteSpace(xmlResponse.ErrorCode) || !string.IsNullOrWhiteSpace(xmlResponse.ErrorMessage)) {
                    _logger.Warn($"При обращении к YandexXml получена ошибка code: {xmlResponse.ErrorCode} message: {xmlResponse.ErrorMessage}");    
                }
                
                if (xmlResponse.Items.Count == 0) {
                    return result;
                }
                
                if (result.Found == 0) {
                    result.Found = xmlResponse.Found;
                }
                
                result.Items.AddRange(xmlResponse.Items);

                if (page == 2) {
                    docOnPage = 50;
                    page = 4;
                }
            } while (result.Items.Count < needResult);

            return result;
        }

        private async Task<string> GetStringContent(string url) {
            var config = _config.GetSection("FileGetter").Get<YandexXmlConfig>();
            
            for (var i = 0; i < config.MaxTryCount; i++) {
                try {
                    WebProxy proxy = null;
                    if (!string.IsNullOrEmpty(config.ProxyUrl) && config.ProxyPort > 0) {
                        proxy = new WebProxy(config.ProxyUrl, config.ProxyPort);
                    }

                    using (var httpClientHandler = new HttpClientHandler {Proxy = proxy}) {
                        using (var client = new HttpClient(httpClientHandler)) {
                            return await client.GetStringAsync(url);
                        }
                    }
                } catch (Exception ex) {
                    _logger.Error(ex, $"При обработке {url} возникло исключение");
                    await Task.Delay(config.ErrorDelayMs);
                }
            }

            return null;
        }

        private YandexXmlResponse Parse(string text) {
            var xml = XDocument.Parse(text);
            var response = xml.Element("yandexsearch")?.Element("response");

            var result = new YandexXmlResponse();
            if (response != null) {
                var error = response.Element("error");
                if (error != null) {
                    result.ErrorCode = error.Attribute("code")?.Value;
                    result.ErrorMessage = error.Value;
                }
                
                foreach (var node in response.Elements("found")) {
                    if (result.Found == 0 && node.Name == "found" && node.Attribute("priority")?.Value == "all") {
                        result.Found = long.Parse(node.Value);
                    }
                }

                var grouping = response.Element("results")?.Element("grouping");
                if (grouping == null) {
                    return result;
                }
                
                foreach (var group in grouping.Elements("group")) {
                    var doc = group.Element("doc");
                    if (doc == null) {
                        continue;
                    }
                    
                    var url = (doc.Element("url")?.Value ?? string.Empty).Trim();
                    var title = (doc.Element("title")?.Value ?? string.Empty).Trim();
                    var headline = (doc.Element("headline")?.Value ?? string.Empty).Trim();
                    var domain = (doc.Element("domain")?.Value ?? string.Empty).Trim();
                            
                    result.Items.Add(new YandexXmlItem {
                        Url = url,
                        Title = title,
                        Domain = domain,
                        Headline = headline
                    });
                }
            }

            return result;
        }
    }
}
