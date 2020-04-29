using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apitron.PDF.Kit;
using Apitron.PDF.Kit.Extraction;
using Apitron.PDF.Kit.FixedLayout;
using Apitron.PDF.Kit.FixedLayout.ContentElements;
using Extractors.Contracts;
using Extractors.Contracts.ContentExtractors;
using Extractors.Contracts.Types;

namespace Extractors.ContentExtractors {
    /// <summary>
    /// Экстрактор текстов из Pdf файлов
    /// </summary>
    public class PdfExtractor : ExtractorBase {
        public PdfExtractor(IContentImageExtractor imageExtractor) : base(imageExtractor) { }
        public override bool IsSupport(string path) {
            return !string.IsNullOrWhiteSpace(path) && path.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase);
        }
        
        /// <summary>
        /// Извлечение текста из документа
        /// </summary>
        /// <param name="bytes">Файл</param>
        /// <param name="extension">Расширение файла</param>
        /// <returns></returns>
        public override DocumentContent ExtractText(byte[] bytes, string extension) {
            var text = new StringBuilder();
            var result = new DocumentContent();

            var iteration = 0;
            try {
                using (var ms = new MemoryStream(bytes)) {
                    using (var doc = new FixedDocument(ms)) {
                        foreach (var page in doc.Pages.Take(2)) {
                            foreach (var element in page.Elements.Where(t => t.ElementType == ElementType.Text || t.ElementType == ElementType.FormXObject)) {
                                if (element.ElementType == ElementType.FormXObject) {
                                    foreach (var formElement in ((FormContentElement)element).Elements.Where(t => t.ElementType == ElementType.Text)) {
                                        text.Append(((TextContentElement) formElement).TextObject?.Text ?? string.Empty);
                                        
                                        if (++iteration > 500) {
                                            result.Content = text.ToString();
                                            return result;
                                        }
                                    }
                                }
                                
                                if (element.ElementType == ElementType.Text) {
                                    text.Append(((TextContentElement) element).TextObject?.Text ?? string.Empty);
                                }

                                if (++iteration > 500) {
                                    result.Content = text.ToString();
                                    return result;
                                }
                            }
                        }
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }

            result.Content = text.ToString();
            return result;
        }
        
        /// <summary>
        /// Извлечение всех картинок из первых двух страниц файла
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        private static IEnumerable<ImageInfo> ExtractImages(Page page) {
            var result = new List<ImageInfo>();

            var iteration = 0;
            foreach (var element in page.Elements.Where(e => e.ElementType == ElementType.Image || e.ElementType == ElementType.FormXObject)) {
                if (++iteration > 500) {
                    break;
                }

                switch (element.ElementType) {
                    case ElementType.FormXObject: {
                        foreach (var formElement in ((FormContentElement)element).Elements.Where(t => t.ElementType == ElementType.Image)) {
                            if (++iteration > 500) {
                                return result;
                            }
                                        
                            result.Add(((ImageContentElement)formElement).ImageInfo);
                        }

                        break;
                    }
                    case ElementType.Image:
                        result.Add(((ImageContentElement) element).ImageInfo);
                        break;
                }
            }

            return result;
        }
        
        /// <summary>
        /// Извлечение текста из картинок в документе
        /// </summary>
        /// <param name="bytes">Файл</param>
        /// <param name="extension">Расширение файла</param>
        /// <returns></returns>
        public override async Task<DocumentContent> ExtractImageText(byte[] bytes, string extension) {
            var result = new DocumentContent();
            var text = new StringBuilder();
            
            try {
                using (var ms = new MemoryStream(bytes)) {
                    using (var doc = new FixedDocument(ms)) {
                        foreach (var page in doc.Pages.Take(2)) {
                            foreach (var image in ExtractImages(page).Where(image => image.EncodedData != null && image.EncodedData.Length != 0)) {
                                var pageText = await _imageExtractor.ExtractTextImage(image.EncodedData);

                                if (string.IsNullOrWhiteSpace(pageText)) {
                                    using (var fs = new MemoryStream()) {
                                        image.SaveToBitmap(fs);
                                        pageText = await _imageExtractor.ExtractTextImage(fs.ToArray());
                                    }
                                }

                                if (!string.IsNullOrWhiteSpace(pageText)) {
                                    text.Append(pageText);
                                }

                                result.HasImageContent = true;
                            }    
                        }
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }

            result.Content = text.ToString();
            return result;
        }
    }
}
