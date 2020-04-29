using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Extractors.Configs;
using Extractors.Types.Document;
using Extractors.Types.Enums;

namespace Extractors.DocumentExtractors {
    /// <summary>
    /// Экстрактор типа файла из текста извлеченного из документа
    /// </summary>
    public class LiteratureExtractor : IDocumentExtractor<LiteratureDocument> {
        private readonly LiteratureExtractorConfig _config;

        private readonly List<Regex> _plusWordsRgxs = new List<Regex>();

        public LiteratureExtractor(LiteratureExtractorConfig config) {
            _config = config;
            if (config.PlusWordsRegexs != null) {
                var rgxs = new List<Regex>();
                foreach (var rgx in config.PlusWordsRegexs) {
                    if (!string.IsNullOrWhiteSpace(rgx)) {
                        rgxs.Add(new Regex(rgx, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant));
                    }
                }

                _plusWordsRgxs = rgxs;
            }
        }
        
        /// <summary>
        /// Извлечение данных из РПД документа
        /// </summary>
        /// <param name="content">РПД документ</param>
        /// <returns></returns>
        public LiteratureDocument Extract(string content) {
            var result = new LiteratureDocument();
            
            if (string.IsNullOrWhiteSpace(content)) {
                return result;
            }
            
            if (_config.MinusWords.Count > 0) {
                foreach (var minusWord in _config.MinusWords) {
                    if (content.Contains(minusWord, StringComparison.InvariantCultureIgnoreCase)) {
                        result.MinusWord = minusWord;
                        return result;
                    }
                } 
            }
            
            if (_config.PlusWords.Count > 0 || _plusWordsRgxs.Count > 0) {
                foreach (var plusWord in _config.PlusWords) {
                    if (content.Contains(plusWord, StringComparison.InvariantCultureIgnoreCase)) {
                        result.PlusWord = plusWord;
                        result.DocumentType = DocumentType.Literature;
                        return result;
                    }
                }    
                
                foreach (var plusWordRegex in _plusWordsRgxs) {
                    var match = plusWordRegex.Match(content);
                    
                    if (match.Success) {
                        result.PlusWord = match.Value;
                        result.DocumentType = DocumentType.Literature;
                        return result;
                    }
                } 
            } else {
                result.DocumentType = DocumentType.Literature;
            }

            return result;
        }
    }
}
