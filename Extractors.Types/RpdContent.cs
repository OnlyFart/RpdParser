using System.Collections.Generic;

namespace Extractors.Types {
    public class RpdContent {
        /// <summary>
        /// Список кодов направлений дисциплин
        /// </summary>
        public List<string> Codes = new List<string>();

        /// <summary>
        /// Признак, является ли файл РПД
        /// </summary>
        public bool IsRpd => Codes.Count > 0;
    }
}
