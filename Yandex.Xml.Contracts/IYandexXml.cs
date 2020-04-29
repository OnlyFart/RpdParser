using System.Threading.Tasks;

namespace Yandex.Xml.Contracts {
    public interface IYandexXmlProvider {
        Task<YandexXmlResponse> Get(string query, int needResult);
    }
}
