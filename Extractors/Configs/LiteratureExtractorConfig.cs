using System.Collections.Generic;

namespace Extractors.Configs {
    public class LiteratureExtractorConfig {
        public List<string> PlusWords { get; set; }
        public List<string> MinusWords { get; set; }
        public List<string> PlusWordsRegexs { get; set; }
    }
}
