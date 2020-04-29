using System.Collections.Generic;

namespace Extractors.Configs {
    /// <summary>
    /// Конфиг для экстрактора списка литературы
    /// </summary>
    public class LiteratureExtractorConfig : ExtractorConfigBase {
        /// <summary>
        /// Список плюс-слов которые будут применены в качестве регулярного выражения
        /// </summary>
        public List<string> PlusWordsRegexs { get; set; }
    }
}
