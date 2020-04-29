namespace Extractors.Configs {
    public class RpdExtractorConfig : ExtractorConfigBase {
        /// <summary>
        /// Регулярное выражение для поиска кода специальности
        /// </summary>
        public string Regex { get; set; }
    }
}
