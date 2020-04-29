using System.IO;
using System.Linq;
using Extractors.Configs;
using Extractors.ContentExtractors;
using Extractors.DocumentExtractors;
using NUnit.Framework;

namespace ExtractorsTests.ContentExtractorsTests {
    public class PdfExtractorsTest {
        [TestCase("Contents/hse.ru_code_exist.pdf", "40.06.01")]
        public void PdfExtractTextTest(string path, string code) {
            var pdfExtractor = new PdfExtractor(new ContentImageExtractor());
            var rdpExtractor = new RpdContentExtractor(new RpdExtractorConfig());
            var bytes = File.ReadAllBytes(path);

            var content = pdfExtractor.ExtractText(bytes, ".pdf");
            var extract = rdpExtractor.Extract(content.Content);
            
            Assert.True(extract.Codes.Count > 0);
            Assert.AreEqual(code, extract.Codes.First());
        }
    }
}
