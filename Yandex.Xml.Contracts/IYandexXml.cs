using System.Threading.Tasks;

namespace Yandex.Xml.Contracts {
    public interface IYandexXml {
        Task<YandexXmlResponse> Get(string query, int needResult);
    }
}
