namespace Extractors.Configs {
    /// <summary>
    /// Сборный объект со всеми конфигами для экстракторов документов
    /// </summary>
    public class DataExtractorConfig {
        /// <summary>
        /// Конфиг для экстрактора РПД документа
        /// </summary>
        public RpdExtractorConfig RpdExtractor { get; set; }
        
        /// <summary>
        /// Конфиг для экстрактора списка литературы
        /// </summary>
        public LiteratureExtractorConfig LiteratureExtractor { get; set; }
    }
}
