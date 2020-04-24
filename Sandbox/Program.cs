using System.Collections.Generic;
using System.IO;
using JRPC.Client;
using Parser.Service.Contracts;

namespace Sandbox {
    class Program {
        static void Main(string[] args) {
            var service = new JRpcClient("http://169.254.173.9:32435/").GetProxy<IParserService>("ParserService");
            
            var rpdFile = new RpdFile {
                Bytes = File.ReadAllBytes("d:\\\\data\\hse.ru\\101.pdf"), 
                Extension = ".pdf",
                Name = "name_1",
                Id = "Id_1",
            };
            
            var rpdFile1 = new RpdFile {
                Bytes = File.ReadAllBytes("d:\\\\data\\hse.ru\\102.pdf"), 
                Extension = ".pdf",
                Name = "name_2",
                Id = "Id_2",
            };

            var list = new List<RpdFile> {rpdFile, rpdFile1};
            
            var rpd = service.ProcessFiles(list);
        }
    }
}
