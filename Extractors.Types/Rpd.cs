namespace Extractors.Types {
    public class Rpd {
        public RpdContent RpdContent = new RpdContent();

        /// <summary>
        /// Путь к сохраненному файлу на диске
        /// </summary>
        public string FilePath;
        
        /// <summary>
        /// Имя файла
        /// </summary>
        public string Name;
        
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
        public bool IsValid = true;
    }
}
