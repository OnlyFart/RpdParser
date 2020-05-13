using FileGetter;
using NUnit.Framework;

namespace FileGetterTests {
    public class FileNameGetterTests {
        [TestCase("", "")]
        [TestCase(null, "")]
        [TestCase("filename*=UTF-8''%D0%A0%D0%9F%D0%A3%D0%94.docx", "РПУД.docx")]
        [TestCase("filename*=UTF-8''%D0%A0%D0%9F%D0%A3%D0%94.docx;  asfsagas", "РПУД.docx")]
        [TestCase("attachment; filename=\"213-422-1-SM.pdf\"", "213-422-1-SM.pdf")]
        [TestCase("attachment; filename=\"213-422-1-SM.pdf\"; sagasgsag", "213-422-1-SM.pdf")]
        [TestCase("attachment; filename=Ñîõðàíåíèå_ïðèðîäíîãî_è_êóëüòóðíîãî_íàñëåäèÿ.doc", "Ñîõðàíåíèå_ïðèðîäíîãî_è_êóëüòóðíîãî_íàñëåäèÿ.doc")]
        [TestCase("attachment; filename=Ñîõðàíåíèå_ïðèðîäíîãî_è_êóëüòóðíîãî_íàñëåäèÿ.doc; safsagg", "Ñîõðàíåíèå_ïðèðîäíîãî_è_êóëüòóðíîãî_íàñëåäèÿ.doc")]
        public void GetTests(string disposition, string expected) {
            Assert.AreEqual(expected, FileNameGetter.Get(disposition));
        }
    }
}
