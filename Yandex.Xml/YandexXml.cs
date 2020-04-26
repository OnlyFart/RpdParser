using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using Yandex.Xml.Contracts;

namespace Yandex.Xml {
    public class YandexXml : IYandexXml {
        private readonly string _user;
        private readonly string _key;

        private const string REQUEST_PATTERN = "https://yandex.com/search/xml?user={0}&key={1}&query={2}&l10n=en&sortby=rlv&filter=none&groupby=attr%3D%22%22.mode%3Dflat.groups-on-page%3D100.docs-in-group%3D1&page={3}";
        private const int MAX_REQUEST_COUNT = 5;
        
        public YandexXml(string user, string key) {
            if (string.IsNullOrWhiteSpace(user)) {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(user));
            }

            if (string.IsNullOrWhiteSpace(key)) {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(key));
            }

            _user = user;
            _key = key;
        }

        public async Task<YandexXmlResponse> Get(string query, int needResult) {
            var result = new YandexXmlResponse();
            var page = 0;
            do {
                var url = string.Format(REQUEST_PATTERN, _user, _key, HttpUtility.UrlEncode(query), page++);
                var response = await GetStringContent(url);
                if (response == null) {
                    return result;
                }

                var xmlResponse = Parse(response);
                if (xmlResponse.Items.Count == 0) {
                    return result;
                }
                
                if (result.Found == 0) {
                    result.Found = xmlResponse.Found;
                }
                
                result.Items.AddRange(xmlResponse.Items);
            } while (result.Items.Count < needResult);

            return result;
        }

        private async Task<string> GetStringContent(string url) {
            for (var i = 0; i < MAX_REQUEST_COUNT; i++) {
                try {
                    using (var client = new HttpClient()) {
                        return await client.GetStringAsync(url);
                    }
                } catch {
                
                }
            }

            return null;
        }

        private YandexXmlResponse Parse(string text) {
            var xml = XDocument.Parse(text);
            var response = xml.Element("yandexsearch")?.Element("response");

            var result = new YandexXmlResponse();
            if (response != null) {
                foreach (var node in response.Elements("found")) {
                    if (result.Found == 0 && node.Name == "found" && node.Attribute("prioryty")?.Value == "all") {
                        result.Found = long.Parse(node.Value);
                    }
                }

                var grouping = response.Element("results")?.Element("grouping");
                if (grouping == null) {
                    return result;
                }
                
                foreach (var group in grouping.Elements("group")) {
                    var doc = @group.Element("doc");
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
