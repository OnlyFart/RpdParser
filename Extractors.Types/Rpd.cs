namespace Extractors.Types {
    public class Rpd {
        public RpdContent RpdContent = new RpdContent();

        /// <summary>
        /// Путь к сохраненному файлу на диске
        /// </summary>
        public string FilePath;
        
        /// <summary>
        /// Путь к файлк на сайте
        /// </summary>
        public string FileUrl;
        
        /// <summary>
        /// Идентификатор файла
        /// </summary>
        public string Id;
        
        /// <summary>
        /// Был ли в файле текст извлеченный из картинок
        /// </summary>
        public bool HasImageContent;

        /// <summary>
        /// Валидный ли это документ
        /// </summary>
        public bool IsValid => string.IsNullOrWhiteSpace(ErrorMessage);
        
        /// <summary>
        /// Сообщение об ошибке, еслм не смогли обработать файл
        /// </summary>
        public string ErrorMessage;
    }
}
