using System;
using System.Linq;
using System.Text.RegularExpressions;
using Extractors.Configs;
using Extractors.Contracts.DocumentExtractors;
using Extractors.Contracts.Enums;
using Extractors.Contracts.Types;

namespace Extractors.DocumentExtractors {
    public class RpdContentExtractor : IDocumentExtractor<RpdDocument> {
        private readonly RpdExtractorConfig _config;
        private readonly Regex _rgx;

        /// <summary>
        /// Экстрактор данных из РПД документа
        /// </summary>
        /// <param name="rgx">Регулярное выражение для поиска номера направления
        /// Внимание!!! В качестве результата будет отдана группа 'code'</param>
        /// <param name="config"></param>
        public RpdContentExtractor(RpdExtractorConfig config) {
            if (config == null) {
                throw new ArgumentNullException(nameof(config));
            }

            if (string.IsNullOrWhiteSpace(config.Regex)) {
                throw new ArgumentNullException(nameof(config.Regex));
            }
            
            _config = config;
            _rgx = new Regex(config.Regex, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }
        
        /// <summary>
        /// Извлечение данных из РПД документа
        /// </summary>
        /// <param name="content">РПД документ</param>
        /// <returns></returns>
        public RpdDocument Extract(string content) {
            var result = new RpdDocument();
            
            if (string.IsNullOrWhiteSpace(content)) {
                return result;
            }

            result.Codes = _rgx.Matches(content).Select(t => t.Groups["code"].Value.Trim()).ToHashSet();
            if (result.Codes.Count > 0) {
                if (_config.MinusWords.Count > 0) {
                    foreach (var minusWord in _config.MinusWords) {
                        if (!string.IsNullOrWhiteSpace(minusWord) && content.Contains(minusWord, StringComparison.InvariantCultureIgnoreCase)) {
                            result.MinusWord = minusWord;
                            result.Success = true;
                            
                            return result;
                        }
                    } 
                }
                
                if (_config.PlusWords.Count > 0) {
                    foreach (var plusWord in _config.PlusWords) {
                        if (!string.IsNullOrWhiteSpace(plusWord) && content.Contains(plusWord, StringComparison.InvariantCultureIgnoreCase)) {
                            result.PlusWord = plusWord;
                            result.DocumentType = DocumentType.Rpd;
                            result.Success = true;
                            
                            break;
                        }
                    }    
                } else {
                    result.DocumentType = DocumentType.Rpd;
                    result.Success = true;
                }
            }
            
            return result;
        }
    }
}
