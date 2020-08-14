using System;
using System.Linq;
using System.Text.RegularExpressions;
using Extractors.Configs;
using Extractors.Contracts.DocumentExtractors;
using Extractors.Contracts.Enums;
using Extractors.Contracts.Types;
using Microsoft.Extensions.Configuration;

namespace Extractors.DocumentExtractors {
    public class RpdContentExtractor : IDocumentExtractor<RpdDocument> {
        private readonly IConfiguration _config;

        /// <summary>
        /// Экстрактор данных из РПД документа
        /// </summary>
        /// <param name="config"></param>
        public RpdContentExtractor(IConfiguration config) {
            if (config == null) {
                throw new ArgumentNullException(nameof(config));
            }

            _config = config;
        }
        
        /// <summary>
        /// Извлечение данных из РПД документа
        /// </summary>
        /// <param name="content">РПД документ</param>
        /// <returns></returns>
        public RpdDocument Extract(string content) {
            var config = _config.GetSection("DataExtractor").Get<DataExtractorConfig>().RpdExtractor;
            var result = new RpdDocument();
            
            if (string.IsNullOrWhiteSpace(content)) {
                return result;
            }

            result.Codes = new Regex(config.Regex, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant).Matches(content).Select(t => t.Groups["code"].Value.Trim()).ToHashSet();
            if (result.Codes.Count > 0) {
                if (config.MinusWords.Count > 0) {
                    foreach (var minusWord in config.MinusWords) {
                        if (!string.IsNullOrWhiteSpace(minusWord) && content.Contains(minusWord, StringComparison.InvariantCultureIgnoreCase)) {
                            result.MinusWord = minusWord;
                            result.Success = true;
                            
                            return result;
                        }
                    } 
                }
                
                if (config.PlusWords.Count > 0) {
                    foreach (var plusWord in config.PlusWords) {
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
