namespace Extractors.Contracts.Types {
    public class DocumentContent {
        /// <summary>
        /// Текст из документа
        /// </summary>
        public string Content = string.Empty;
        
        /// <summary>
        /// Была ли информация извлечена из картинок
        /// </summary>
        public bool HasImageContent;
    }
}
