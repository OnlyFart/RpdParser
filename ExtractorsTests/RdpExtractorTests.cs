using Extractors.DataExtractors;
using NUnit.Framework;

namespace ExtractorsTests {
    public class RdpExtractorTests {
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
        [TestCase("11.03.24 trutrutru 11.03.24  trutrututr  11.03.24rtutru rtu 1 ", 3)]
        [TestCase("11.03.24\r 11.03.24\r 11.03.241 ", 2)]
        public void ExtractCodesTest(string content, int count) {
            var extractor = new RpdContentExtractor();
            Assert.AreEqual(count, extractor.Extract(content).Codes.Count);
        }
    }
}
