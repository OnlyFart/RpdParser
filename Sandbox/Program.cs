using FileGetter;

namespace Sandbox {
    class Program {
        static void Main(string[] args) {
            var pg = new FileNetworkGetter();
            var file = pg.GetFile("http://physics.tsu.tula.ru/bib/programmy/RP/RP_Fizika_034300.pdf").Result;
        }
    }
}
