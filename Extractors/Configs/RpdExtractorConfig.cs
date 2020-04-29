using System.Collections.Generic;

namespace Extractors.Configs {
    public class RpdExtractorConfig {
        public List<string> PlusWords { get; set; }
        public List<string> MinusWords { get; set; }
        public string Regex { get; set; }
    }
}
