using System.Collections.Generic;
using System.Linq;

namespace Extractors.Configs {
    /// <summary>
    /// Базовый конфиг экстрактора
    /// </summary>
    public class ExtractorConfigBase {
        private List<string> _plusWords;
        private List<string> _minusWords;

        /// <summary>
        /// Плюс-слова для поиска в документа
        /// </summary>
        public List<string> PlusWords  {
            get => _plusWords; 
            set { _plusWords = value.Select(w => w.Replace(" ", "")).ToList(); }
        }

        /// <summary>
        /// Минус слова для поиска в документе
        /// </summary>
        public List<string> MinusWords  {
            get => _minusWords; 
            set { _minusWords = value.Select(w => w.Replace(" ", "")).ToList(); }
        }

        protected ExtractorConfigBase(IEnumerable<string> plusWords, IEnumerable<string> minusWords) {
            PlusWords = plusWords.ToList();
            MinusWords = minusWords.ToList();
        }
    }
}
