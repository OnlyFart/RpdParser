using System.Collections.Generic;
using Extractors.Configs;
using Extractors.Contracts.Enums;
using Extractors.DocumentExtractors;
using NUnit.Framework;

namespace ExtractorsTests.DocumentExtractorsTests {
    public class RdpExtractorTests {
        private const string REGEX = @"(?<code>\d\d\.0\d\.\d\d)($|\D)";

        [TestCase("11.03.24", 1)]
        [TestCase("ф11.03.24", 1)]
        [TestCase("gs11.03.24sdg", 1)]
        [TestCase("11.03.24\r", 1)]
        [TestCase("11.03.24\n", 1)]
        [TestCase("11.03.24 ", 1)]
        [TestCase("ф11.03.24 ", 1)]
        [TestCase("11.13.24", 0)]
        [TestCase("11.03.2011", 0)]
        [TestCase("11.1а.24", 0)]
        [TestCase("11.03.241", 0)]
        [TestCase("11.13.24f", 0)]
        [TestCase("11.13.24f 11.03.24", 1)]
        [TestCase("11.13.24f 11.03.24 11.03.25", 2)]
        [TestCase("11.03.24 trutrutru 11.03.24  trutrututr  11.03.24rtutru rtu 1 ", 1)]
        [TestCase("11.03.24\r 11.03.24\r 11.03.241 ", 1)]
        public void ExtractCodesTest(string content, int count) {
            var rpdExtractorConfig = new RpdExtractorConfig(new List<string>(), new List<string>(), REGEX);
            var extractor = new RpdContentExtractor(rpdExtractorConfig);
            Assert.AreEqual(count, extractor.Extract(content).Codes.Count);
        }
        
        [TestCase("11.03.24", "плюс", DocumentType.Unknown)]
        [TestCase("ф11.03.24 плюс", "плюс", DocumentType.Rpd)]
        [TestCase("ф11.034 плюс", "плюс", DocumentType.Unknown)]
        public void ExtractPlusWordsTest(string content, string plus, DocumentType type) {
            var rpdExtractorConfig = new RpdExtractorConfig(new List<string>{plus}, new List<string>(), REGEX);
            var extractor = new RpdContentExtractor(rpdExtractorConfig);
            Assert.AreEqual(type, extractor.Extract(content).DocumentType);
        }
        
        [TestCase("11.03.24", "минус", DocumentType.Rpd)]
        [TestCase("ф11.03.24 минус", "минус", DocumentType.Unknown)]
        [TestCase("ф11.034 минус", "минус", DocumentType.Unknown)]
        public void ExtractMinusWordsTest(string content, string minus, DocumentType type) {
            var rpdExtractorConfig = new RpdExtractorConfig(new List<string>(), new List<string>{minus}, REGEX);
            var extractor = new RpdContentExtractor(rpdExtractorConfig);
            Assert.AreEqual(type, extractor.Extract(content).DocumentType);
        }

        [TestCase("11.03.24", "плюс", "минус", DocumentType.Unknown)]
        [TestCase("ф11.03.24 минус", "плюс", "минус", DocumentType.Unknown)]
        [TestCase("ф11.034 плюс минус", "плюс", "минус", DocumentType.Unknown)]
        [TestCase("11.03.24 плюс", "плюс", "минус", DocumentType.Rpd)]
        public void ExtractPlusMinusWordsTest(string content, string plus, string minus, DocumentType type) {
            var rpdExtractorConfig = new RpdExtractorConfig(new List<string>{plus}, new List<string>{minus}, REGEX);
            var extractor = new RpdContentExtractor(rpdExtractorConfig);
            Assert.AreEqual(type, extractor.Extract(content).DocumentType);
        }
    }
}
