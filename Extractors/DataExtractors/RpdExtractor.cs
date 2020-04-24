using System.Linq;
using System.Text.RegularExpressions;
using Extractors.Types;

namespace Extractors.DataExtractors {
    public class RpdContentExtractor {
        /// <summary>
        /// Регулярное выражение, используемое по умолчанию для поиска
        /// </summary>
        private readonly Regex _default = new Regex(@"(?<code>\d\d\.0\d\.\d\d)($|\D)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        
        /// <summary>
        /// Регулярное выражение которое перекрывает, выражение по умолчанию
        /// </summary>
        private Regex _custom;
        
        public Regex Rgx  {
            get => _custom ?? _default;
            set => _custom = value;
        }
        
        /// <summary>
        /// Экстрактор данных из РПД документа
        /// </summary>
        public RpdContentExtractor() {

        }
        
        /// <summary>
        /// Экстрактор данных из РПД документа
        /// </summary>
        /// <param name="rgx">Регулярное выражение для поиска номера направления
        /// Внимание!!! В качестве результата будет отдана группа 'code'</param>
        public RpdContentExtractor(Regex rgx) {
            Rgx = rgx;
        }
        
        /// <summary>
        /// Извлечение данных из РПД документа
        /// </summary>
        /// <param name="content">РПД документ</param>
        /// <returns></returns>
        public RpdContent Extract(string content) {
            var result = new RpdContent();
            
            if (string.IsNullOrWhiteSpace(content)) {
                return result;
            }

            result.Codes = Rgx.Matches(content).Select(t => t.Groups["code"].Value.Trim()).ToList();
            return result;
        }
    }
}
