namespace FileGetter.Configs {
    public class FileGetterConfig {
        /// <summary>
        /// Максимально кол-во попыток в случае транспортных ошибок
        /// </summary>
        public int MaxTryCount { get; set; }
        
        /// <summary>
        /// Задержка в миллисекундах между ошибосными запросами
        /// </summary>
        public int ErrorDelayMs { get; set; }
    }
}
