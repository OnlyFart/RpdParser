using System.Collections.Generic;

namespace Extractors.Configs {
    public class RpdExtractorConfig : ExtractorConfigBase {
        /// <summary>
        /// Регулярное выражение для поиска кода специальности
        /// </summary>
        public string Regex { get; set; }

        public RpdExtractorConfig() : this(new List<string>(), new List<string>(), string.Empty) {
            
        }

        public RpdExtractorConfig(IEnumerable<string> plusWords, IEnumerable<string> minusWords, string regex) : base(plusWords, minusWords) {
            Regex = regex;
        }
    }
}
