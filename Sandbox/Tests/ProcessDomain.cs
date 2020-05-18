using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JRPC.Client;
using Parser.Service.Contracts.Service;

namespace Sandbox.Tests {
    public static class ProcessDomains {
        public static void Process(IParserService parser) {
            var lines = File.ReadAllLines("test_domains.txt");
            var index = 0;
            Parallel.ForEach(lines, new ParallelOptions {MaxDegreeOfParallelism = 5}, domain => {
                var sw = Stopwatch.StartNew();
                Interlocked.Increment(ref index);

                try {
                    var result = parser.ProcessFilesByDomain(domain).Result;
                    Console.WriteLine($"{index} {domain} {result.Count()} {TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds)}");
                } catch (Exception e) {
                    Console.WriteLine(e);
                }
            });
        }
    }
}
