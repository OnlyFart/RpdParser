using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aspose.Words;
using Aspose.Words.Drawing;
using Aspose.Words.Saving;
using Extractors.ContentExtractors.ContentImageExtractors;
using Extractors.Types;
using Document = Aspose.Words.Document;

namespace Extractors.ContentExtractors {
    public class DocExtractor : ExtractorBase {

        public DocExtractor(IContentImageExtractor imageExtractor) : base(imageExtractor) { }

        /// <summary>
        /// Поддерживаемый форматы файлов для обработки
        /// Библиотека поддерживает и больше. Добавил пока только эти
        /// В случае необходимости обработки новых форматов просто докидывайте сюда
        /// В теории должно работать. Сюда можно добавить еще и .pdf и это будет работать
        /// что позволит избавиться от PdfExtractor, но почему то эта библиотека очень
        /// медленно обрабатывает PDF, т.ч. пусть этим занимается PdfExtractor
        /// </summary>
        private static readonly Dictionary<string, LoadFormat> _supportedExtension = new Dictionary<string, LoadFormat> {
            {".docx", LoadFormat.Docx},
            {".doc", LoadFormat.Doc},
            {".rtf", LoadFormat.Rtf},
            {".odt", LoadFormat.Odt},
        };

        /// <summary>
        /// Проверка на поддержку расширения данным экстрактором
        /// </summary>
        public override bool IsSupport(string path) {
            return !string.IsNullOrWhiteSpace(path) && _supportedExtension.Any(extension => path.EndsWith(extension.Key, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Определение типа файла по его расширению
        /// </summary>
        private static LoadFormat GetLoadFormat(string path) {
            foreach (var extension in _supportedExtension) {
                if (path.EndsWith(extension.Key, StringComparison.InvariantCultureIgnoreCase)) {
                    return extension.Value;
                }
            }

            throw new ArgumentException($"Unsupported file type {path}");
        }

        public override DocumentContent ExtractText(byte[] bytes, string extension) {
            var content = new StringBuilder();
            var result = new DocumentContent();

            try {
                using (var ms = new MemoryStream(bytes)) {
                    var doc = new Document(ms, new LoadOptions(GetLoadFormat(extension), null, null));
                    var paragraphs = doc.GetChildNodes(NodeType.Paragraph, true);

                    //NOTE: Не всегда получает только первую страницу. Довольно часто цепляет еще несколько
                    foreach (var node in paragraphs) {
                        var text = node.GetText();

                        if (text.Contains(ControlChar.PageBreak)) {
                            var index = text.IndexOf(ControlChar.PageBreak, StringComparison.Ordinal);
                            content.Append(text.Substring(0, index + 1));
                            break;
                        }

                        content.Append(text);
                    }
                }
            } catch { }

            result.Content = content.ToString();
            return result;
        }

        public override async Task<DocumentContent> ExtractImageText(byte[] bytes, string extension) {
            var content = new StringBuilder();
            var result = new DocumentContent();

            try {
                using (var ms = new MemoryStream(bytes)) {
                    var doc = new Document(ms, new LoadOptions(GetLoadFormat(extension), null, null));

                    foreach (var node in doc.GetChildNodes(NodeType.Shape, true)) {
                        var shape = (Shape) node;
                        if (!shape.HasImage || shape.ImageData.ImageBytes.Length == 8818) {
                            continue;
                        }

                        string extractTextImage;
                        if (shape.ImageData.ImageType == ImageType.Emf) {
                            using (var imageBytes = new MemoryStream()) {
                                shape.GetShapeRenderer().Save(imageBytes, new ImageSaveOptions(SaveFormat.Png));
                                extractTextImage = await _imageExtractor.ExtractTextImage(imageBytes.ToArray());
                            }
                        } else {
                            extractTextImage = await _imageExtractor.ExtractTextImage(shape.ImageData.ImageBytes);
                            
                        }
                        
                        if (string.IsNullOrWhiteSpace(extractTextImage)) {
                            var image = shape.ImageData.ToImage();
                            extractTextImage = await _imageExtractor.ExtractTextImage(image.Bytes);
                        }
                        
                        content.Append(extractTextImage);
                        result.HasImageContent = true;
                    }
                }
            } catch { }

            result.Content = content.ToString();
            return result;
        }
    }
}