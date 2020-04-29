using System.Collections.Generic;
using Extractors.Configs;
using Extractors.Contracts.Enums;
using Extractors.DocumentExtractors;
using NUnit.Framework;

namespace ExtractorsTests.DocumentExtractorsTests {
    public class LiteratureExtractorTests {
        [Test]
        public void ExtractEmptyTest() {
            var config = new LiteratureExtractorConfig(new List<string>(), new List<string>(), new List<string>());
            var extractor = new LiteratureExtractor(config);
            Assert.AreEqual(DocumentType.Unknown, extractor.Extract("test").DocumentType);
        }
        
        [TestCase("плюс dfgdfh", "плюс", DocumentType.Literature)]
        [TestCase("ф11.034", "плюс", DocumentType.Unknown)]
        public void ExtractPlusWordsTest(string content, string plus, DocumentType type) {
            var config = new LiteratureExtractorConfig(new List<string>{plus}, new List<string>(), new List<string>());
            var extractor = new LiteratureExtractor(config);
            Assert.AreEqual(type, extractor.Extract(content).DocumentType);
        }
        
        [TestCase("45 плюс dfgdfh", "\\d+", DocumentType.Literature)]
        [TestCase("ф11.03", "\\d\\d\\d", DocumentType.Unknown)]
        public void ExtractPlusWordsRegexTest(string content, string plus, DocumentType type) {
            var config = new LiteratureExtractorConfig(new List<string>{plus}, new List<string>{plus}, new List<string>());
            var extractor = new LiteratureExtractor(config);
            Assert.AreEqual(type, extractor.Extract(content).DocumentType);
        }
        
        [TestCase("плюс", "минус", DocumentType.Unknown)]
        [TestCase("плюс минус", "минус", DocumentType.Unknown)]
        [TestCase("", "минус", DocumentType.Unknown)]
        public void ExtractMinusWordsTest(string content, string minus, DocumentType type) {
            var config = new LiteratureExtractorConfig(new List<string>(), new List<string>(), new List<string>{minus});
            var extractor = new LiteratureExtractor(config);
            Assert.AreEqual(type, extractor.Extract(content).DocumentType);
        }

        [TestCase("11.03.24", "плюс", "минус", DocumentType.Unknown)]
        [TestCase("ф11.03.24 минус", "плюс", "минус", DocumentType.Unknown)]
        [TestCase("ф11.034 плюс минус", "плюс", "минус", DocumentType.Unknown)]
        [TestCase("11.03.24 плюс", "плюс", "минус", DocumentType.Literature)]
        public void ExtractPlusMinusWordsTest(string content, string plus, string minus, DocumentType type) {
            var config = new LiteratureExtractorConfig(new List<string>{plus}, new List<string>(), new List<string>{minus});
            var extractor = new LiteratureExtractor(config);
            
            var literatureDocument = extractor.Extract(content);
            Assert.AreEqual(type, literatureDocument.DocumentType);
        }
    }
}
