using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using JRPC.Client;
using Parser.Service.Contracts.Service;

namespace Sandbox.Tests {
    public static class ProcessDomains {
        public static void Process(IParserService parser) {
            var lines = File.ReadAllLines("test_domains.txt");
            for (var index = 0; index < lines.Length; index++) {
                var sw = Stopwatch.StartNew();
                var domain = lines[index];

                try {
                    var result = parser.ProcessFilesByDomain(domain).Result;
                    Console.WriteLine($"{index + 1} {domain} {result.Count()} {TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds)}");
                } catch (Exception e) {
                    Console.WriteLine(e);
                }
            }
        }
    }
}
