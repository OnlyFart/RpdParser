using System.Collections.Generic;

namespace Yandex.Xml.Contracts {
    public class YandexXmlResponse {
        public long Found;
        public List<YandexXmlItem> Items = new List<YandexXmlItem>();
    }
}
