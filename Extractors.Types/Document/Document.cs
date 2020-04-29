namespace Extractors.Types.Document {
    public class Document {
        public DocumentBase DocumentContent = new DocumentBase();

        /// <summary>
        /// Путь к сохраненному файлу на диске
        /// </summary>
        public string FilePath;
        
        /// <summary>
        /// Путь к файлу на сайте
        /// </summary>
        public string FileUrl;

        /// <summary>
        /// Был ли в файле текст извлеченный из картинок
        /// </summary>
        public bool HasImageContent;

        /// <summary>
        /// Валидный ли это документ
        /// </summary>
        public bool IsValid => string.IsNullOrWhiteSpace(ErrorMessage);
        
        /// <summary>
        /// Сообщение об ошибке, если не удалось обработать файл
        /// </summary>
        public string ErrorMessage;
    }
}
