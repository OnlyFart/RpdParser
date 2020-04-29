using System.Threading.Tasks;
using Yandex.Xml.Contracts.Types;

namespace Yandex.Xml.Contracts {
    public interface IYandexXmlProvider {
        Task<YandexXmlResponse> Get(string query, int needResult);
    }
}
