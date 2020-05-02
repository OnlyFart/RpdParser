using System.Collections.Generic;
using System.IO;
using System.Linq;
using Extractors.Configs;
using Extractors.ContentExtractors;
using Extractors.DocumentExtractors;
using NUnit.Framework;

namespace ExtractorsTests.ContentExtractorsTests {
    public class DocExtractorsTests {
        [TestCase("Contents/hse.ru_code_exist.doc", "38.04.02")]
        [TestCase("Contents/hse.ru_code_exist.docx", "38.04.04")]
        public void DocExtractTextTest(string path, string code) {
            var extractor = new DocExtractor(new ContentImageExtractor());
            var rdpExtractor = new RpdContentExtractor(new RpdExtractorConfig(new List<string>(), new List<string>(), @"(?<code>\d\d\.0\d\.\d\d)($|\D)"));
            var bytes = File.ReadAllBytes(path);

            var content = extractor.ExtractText(bytes, path);
            var extract = rdpExtractor.Extract(content.Content);
            
            Assert.True(extract.Codes.Count > 0);
            Assert.AreEqual(code, extract.Codes.First());
        }
    }
}
