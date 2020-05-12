using FileGetter;
using NUnit.Framework;

namespace FileGetterTests {
    public class FileNameGetterTests {
        [TestCase("", "")]
        [TestCase(null, "")]
        [TestCase("filename*=UTF-8''%D0%A0%D0%9F%D0%A3%D0%94.docx", "РПУД.docx")]
        public void GetTests(string disposition, string expected) {
            Assert.AreEqual(expected, FileNameGetter.Get(disposition));
        }
    }
}
