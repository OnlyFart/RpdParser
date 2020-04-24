using System;
using System.IO;
using System.Text;
using Extractors.Types;
using GemBox.Pdf;
using GemBox.Pdf.Content;

namespace Extractors.ContentExtractors {
    public class PdfExtractorBase : ExtractorBase {
        /// <summary>
        /// Проверка на поддержку файла данным экстрактором
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns></returns>
        public override bool IsSupport(string path) {
            return !string.IsNullOrWhiteSpace(path) && path.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Извлечение текста из документа
        /// </summary>
        /// <param name="bytes">Файл</param>
        /// <param name="extension">Расширение файла</param>
        /// <returns></returns>
        public override Extract Extract(byte[] bytes, string extension) {
            var text = new StringBuilder();
            var result = new Extract();
            
            try {
                using (var ms = new MemoryStream(bytes)) {
                    using (var document = PdfDocument.Load(ms)) {
                        if (document.Pages.Count == 0) {
                            return result;
                        }

                        foreach (var contentElement in document.Pages[0].Content.Elements.All()) {
                            switch (contentElement.ElementType) {
                                case PdfContentElementType.Text:
                                    text.Append((PdfTextContent) contentElement);
                                    break;
                                case PdfContentElementType.Image:
                                    var imageContent = GetImageContent((PdfImageContent) contentElement);
                                    if (!string.IsNullOrWhiteSpace(imageContent)) {
                                        text.Append(imageContent);
                                        result.HasImageContent = true;
                                    }

                                    break;
                            }

                        }
                    }
                }
            } catch {
            }

            result.Content = text.ToString();
            return result;
        }

        private static string GetImageContent(PdfImageContent imageContent) {
            var text = string.Empty;

            try {
                using (var ms = new MemoryStream()) {
                    imageContent.Save(ms, new ImageSaveOptions(ImageSaveFormat.Jpeg));
                    text = ExtractTextImage(ms.ToArray());
                }
            } catch {
                
            }

            return text;
        }
    }
}
