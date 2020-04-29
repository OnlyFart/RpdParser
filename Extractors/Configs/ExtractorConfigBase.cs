using System.Collections.Generic;

namespace Extractors.Configs {
    /// <summary>
    /// Базовый конфиг экстрактора
    /// </summary>
    public class ExtractorConfigBase {
        /// <summary>
        /// Плюс-слова для поиска в документа
        /// </summary>
        public List<string> PlusWords { get; set; }
        
        /// <summary>
        /// Минус слова для поиска в документе
        /// </summary>
        public List<string> MinusWords { get; set; }
    }
}
