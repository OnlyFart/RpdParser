using System;
using System.Threading;
using JRPC.Service;
using Ninject;
using Parser.Service.IOC;

namespace Parser.Service {
    class Program {
        static void Main(string[] args) {
            var standardKernel = new StandardKernel(new ParserServiceNinjectModule());
            standardKernel.Get<JRpcService>().Start();
            WaitForCancel();
        }
        
        private static void WaitForCancel() {
            var @event = new ManualResetEventSlim(false);
            Console.CancelKeyPress += (s, e) => {
                Console.WriteLine("Ctrl+C Pressed");
                @event.Set();
            };

            @event.Wait();
        }
    }
}
