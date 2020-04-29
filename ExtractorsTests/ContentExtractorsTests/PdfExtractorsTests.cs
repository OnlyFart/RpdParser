using System.Collections.Generic;
using System.IO;
using System.Linq;
using Extractors.Configs;
using Extractors.ContentExtractors;
using Extractors.Contracts.Enums;
using Extractors.DocumentExtractors;
using NUnit.Framework;

namespace ExtractorsTests.ContentExtractorsTests {
    public class PdfExtractorsTest {
        [TestCase("Contents/hse.ru_code_exist.pdf", "рабочая программа", "40.06.01")]
        [TestCase("Contents/ioe.hse.ru_code_exist.pdf", "рабочая программа", "44.06.01")]
        [TestCase("Contents/logistic.hse.ru_code_exits.pdf", "рабочая программа", "38.04.02")]
        [TestCase("Contents/www.hse.ru_code_exist.pdf", "рабочая программа", "45.03.02")]
        public void PdfExtractTextRpdTest(string path, string plus, string code) {
            var pdfExtractor = new PdfExtractor(new ContentImageExtractor());
            var rdpExtractor = new RpdContentExtractor(new RpdExtractorConfig(new List<string>{plus}, new List<string>()));
            var bytes = File.ReadAllBytes(path);

            var content = pdfExtractor.ExtractText(bytes, ".pdf");
            var extract = rdpExtractor.Extract(content.Content);
            
            Assert.True(extract.Codes.Count > 0);
            Assert.AreEqual(code, extract.Codes.First());
            Assert.AreEqual(DocumentType.Rpd, extract.DocumentType);
        }
    }
}
