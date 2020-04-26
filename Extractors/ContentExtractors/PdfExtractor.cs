using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extractors.ContentExtractors.ContentImageExtractors;
using Extractors.Types;
using GemBox.Pdf;
using GemBox.Pdf.Content;

namespace Extractors.ContentExtractors {
    public class PdfExtractor : ExtractorBase {
        public PdfExtractor(IContentImageExtractor imageExtractor) : base(imageExtractor) { }
        
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
        public override Extract ExtractText(byte[] bytes, string extension) {
            var text = new StringBuilder();
            var result = new Extract();

            try {
                using (var ms = new MemoryStream(bytes)) {
                    using (var document = PdfDocument.Load(ms)) {
                        var page = document.Pages.FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.Content.ToString()));
                        if (page == null) {
                            return result;
                        }
                        
                        text.Append(page.Content);
                    }
                }
            } catch { }

            result.Content = text.ToString();
            return result;
        }

        public override async Task<Extract> ExtractImageText(byte[] bytes, string extension) {
            var text = new StringBuilder();
            var result = new Extract();

            try {
                using (var ms = new MemoryStream(bytes)) {
                    using (var document = PdfDocument.Load(ms)) {
                        if (document.Pages.Count == 0) {
                            return result;
                        }

                        foreach (var contentElement in document.Pages[0].Content.Elements.All().Where(t => t.ElementType == PdfContentElementType.Image)) {
                            var imageContent = await GetImageContent((PdfImageContent) contentElement);
                            if (!string.IsNullOrWhiteSpace(imageContent)) {
                                text.Append(imageContent);
                                result.HasImageContent = true;
                            }
                        }
                    }
                }
            } catch { }

            result.Content = text.ToString();
            return result;
        }

        private async Task<string> GetImageContent(PdfImageContent imageContent) {
            var text = string.Empty;

            try {
                using (var ms = new MemoryStream()) {
                    imageContent.Save(ms, new ImageSaveOptions(ImageSaveFormat.Jpeg));
                    text = await _imageExtractor.ExtractTextImage(ms.ToArray());
                }
            } catch {
                
            }

            return text;
        }
    }
}
