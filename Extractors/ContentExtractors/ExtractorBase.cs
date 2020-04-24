using Extractors.Types;
using Tesseract;

namespace Extractors.ContentExtractors {
    public abstract class ExtractorBase {
        /// <summary>
        /// Проверка на поддержку файла данным экстрактором
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns></returns>
        public abstract bool IsSupport(string path);

        /// <summary>
        /// Извлечение текста из документа
        /// </summary>
        /// <param name="bytes">Файл</param>
        /// <param name="extension">Расширение файла</param>
        /// <returns></returns>
        public abstract Extract Extract(byte[] bytes, string extension);
        
        /// <summary>
        /// Извлечение текста из картинки
        /// </summary>
        /// <param name="image">Картинка в виде массива байт</param>
        /// <returns></returns>
        protected static string ExtractTextImage(byte[] image) {
            var result = string.Empty;

            try {
                using (var engine = new TesseractEngine(@"./tessdata", "rus", EngineMode.Default)) {
                    using (var img = Pix.LoadFromMemory(image)) {
                        using (var process = engine.Process(img)) {
                            result = process.GetText();
                        }
                    }
                }
            } catch { }

            return result;
        }
    }
}