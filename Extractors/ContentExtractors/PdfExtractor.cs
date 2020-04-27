using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Apitron.PDF.Kit;
using Extractors.ContentExtractors.ContentImageExtractors;
using Extractors.Types;

namespace Extractors.ContentExtractors {
    public class PdfExtractor : ExtractorBase {
        public PdfExtractor(IContentImageExtractor imageExtractor) : base(imageExtractor) { }
        public override bool IsSupport(string path) {
            return !string.IsNullOrWhiteSpace(path) && path.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase);
        }

        public override Extract ExtractText(byte[] bytes, string extension) {
            var text = new StringBuilder();
            var result = new Extract();

            try {
                using (var ms = new MemoryStream(bytes)) {
                    using (var doc = new FixedDocument(ms)) {
                        foreach (var page in doc.Pages) {
                            var pageText = page.ExtractText().Trim();
                            if (!string.IsNullOrWhiteSpace(pageText)) {
                                text.Append(pageText);
                                break;
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

        public override async Task<Extract> ExtractImageText(byte[] bytes, string extension) {
            var result = new Extract();

            try {
                using (var ms = new MemoryStream(bytes)) {
                    using (var doc = new FixedDocument(ms)) {
                        if (doc.Pages.Count == 0) {
                            return result;
                        }

                        foreach (var image in doc.Pages[0].ExtractImages()) {
                            var pageText = await _imageExtractor.ExtractTextImage(image.EncodedData);
                            if (string.IsNullOrWhiteSpace(pageText)) {
                                continue;
                            }

                            result.Content = pageText;
                            result.HasImageContent = true;
                            break;
                        }
                    }
                }
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }

            return result;
        }
    }
}
