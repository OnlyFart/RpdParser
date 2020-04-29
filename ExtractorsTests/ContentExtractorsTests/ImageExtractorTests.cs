using System.IO;
using System.Linq;
using Extractors.Configs;
using Extractors.ContentExtractors;
using Extractors.DocumentExtractors;
using NUnit.Framework;

namespace ExtractorsTests.ContentExtractorsTests {
    public class ImageExtractorTests {
        [TestCase("Contents/hse.ru_code_image_exist.docx", "38.03.02")]
        public void ImageExtractDocTextTest(string path, string code) {
            var extractor = new DocExtractor(new ContentImageExtractor());
            var rdpExtractor = new RpdContentExtractor(new RpdExtractorConfig());
            var bytes = File.ReadAllBytes(path);

            var content = extractor.ExtractImageText(bytes, path).Result;
            var extract = rdpExtractor.Extract(content.Content);
            
            Assert.True(extract.Codes.Count > 0);
            Assert.AreEqual(code, extract.Codes.First());
        }
        
        [TestCase("Contents/rachmaninov.ru_code_exist.pdf", "53.02.03")]
        public void ImageExtractPdfTextTest(string path, string code) {
            var extractor = new PdfExtractor(new ContentImageExtractor());
            var rdpExtractor = new RpdContentExtractor(new RpdExtractorConfig());
            var bytes = File.ReadAllBytes(path);

            var content = extractor.ExtractImageText(bytes, path).Result;
            var extract = rdpExtractor.Extract(content.Content);
            
            Assert.True(extract.Codes.Count > 0);
            Assert.AreEqual(code, extract.Codes.First());
        }
    }
}
