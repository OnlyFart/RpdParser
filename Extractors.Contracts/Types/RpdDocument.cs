using System.Collections.Generic;

namespace Extractors.Contracts.Types {
    public class RpdDocument : DocumentBase {
        /// <summary>
        /// Список кодов направлений дисциплин
        /// </summary>
        public HashSet<string> Codes = new HashSet<string>();
        
        /// <summary>
        /// Найденное плюс-слово
        /// </summary>
        public string PlusWord;
        
        /// <summary>
        /// Найденное минус-слово
        /// </summary>
        public string MinusWord;

        public override string LogMessage() {
            return $"Плюс-слово: [{PlusWord}] "
                   + $"Минус-слово: [{MinusWord}] "
                   + $"Найденные коды: [{string.Join(", ", Codes)}]";
        }
    }
}
