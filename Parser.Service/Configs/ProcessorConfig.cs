namespace Parser.Service.Configs {
    /// <summary>
    /// Конфиг процессова
    /// </summary>
    public class ProcessorConfig {
        /// <summary>
        /// Максимальное количесво потовок для обработки одного запроса
        /// </summary>
        public int MaxParallelThreads { get; set; }
        
        /// <summary>
        /// Директория, в которую будут сохраняться скачанные файлы
        /// </summary>
        public string BaseDirectory { get; set; }
        
        /// <summary>
        /// Шаблоны запросов для отправки в Yandex.Xml
        /// </summary>
        public string[] XmlPatterns { get; set; }
    }
}
