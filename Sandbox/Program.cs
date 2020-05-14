using JRPC.Client;
using Parser.Service.Contracts.Service;
using Sandbox.Tests;

namespace Sandbox {
    class Program {
        static void Main(string[] args) {
            var parser = new JRpcClient("http://169.254.99.73:32435/").GetProxy<IParserService>("ParserService");
            
            ProcessDomains.Process(parser);
        }
    }
}
