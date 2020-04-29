using System.Collections.Generic;
using System.Linq;

namespace Extractors.Configs {
    /// <summary>
    /// Конфиг для экстрактора списка литературы
    /// </summary>
    public class LiteratureExtractorConfig : ExtractorConfigBase {
        /// <summary>
        /// Список плюс-слов которые будут применены в качестве регулярного выражения
        /// </summary>
        public List<string> PlusWordsRegexs { get; }

        public LiteratureExtractorConfig() : this(new List<string>(), new List<string>(), new List<string>()) {
            
        }
        
        public LiteratureExtractorConfig(IEnumerable<string> plusWords, IEnumerable<string> plusWordsRegexs, IEnumerable<string> minusWords) : base(plusWords, minusWords) {
            PlusWordsRegexs = plusWordsRegexs.ToList();
        }
    }
}
