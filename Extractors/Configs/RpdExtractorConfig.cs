using System.Collections.Generic;

namespace Extractors.Configs {
    public class RpdExtractorConfig : ExtractorConfigBase {
        /// <summary>
        /// Регулярное выражение для поиска кода специальности
        /// </summary>
        public string Regex { get; set; }

        public RpdExtractorConfig() : this(new List<string>(), new List<string>()) {
            
        } 
        public RpdExtractorConfig(IEnumerable<string> plusWords, IEnumerable<string> minusWords) : base(plusWords, minusWords) { }
    }
}
