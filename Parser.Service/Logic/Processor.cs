using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Extractors.ContentExtractors;
using Extractors.DataExtractors;
using Extractors.Types;
using Parser.Service.Contracts;

namespace Parser.Service.Logic {
    public class Processor {
        private readonly List<ExtractorBase> _extractors;
        private readonly RpdContentExtractor _rpdContentExtractor;

        public Processor(List<ExtractorBase> extractors, RpdContentExtractor rpdContentExtractor) {
            _extractors = extractors;
            _rpdContentExtractor = rpdContentExtractor;
        }
        
        public Rpd ProcessFile(string path) {
            var bytes = File.ReadAllBytes(path);
            var rpd = ProcessFile(bytes, path);
            
            rpd.FilePath = path;
            
            return rpd;
        }
        
        public Rpd ProcessFile(RpdFile file) {
            var rpd = ProcessFile(file.Bytes, file.Extension);
            
            rpd.Id = file.Id;
            rpd.Name = file.Name;
            
            return rpd;
        }

        public IEnumerable<Rpd> Process(IEnumerable<string> paths) {
            var queue = new ConcurrentQueue<Rpd>();
            Parallel.ForEach(paths, new ParallelOptions {MaxDegreeOfParallelism = 50}, path => {
                var rpd = ProcessFile(path);
                
                if (rpd.RpdContent.Codes.Count == 0) {
                    Console.WriteLine("FAIL " + path);
                }
						
                Console.WriteLine("SUCCESS " + path + " " + string.Join(", ", rpd.RpdContent.Codes.Distinct()));
                queue.Enqueue(rpd);
            });

            return queue.ToList();
        }
        
        public IEnumerable<Rpd> Process(IEnumerable<RpdFile> files) {
            var queue = new ConcurrentQueue<Rpd>();
            Parallel.ForEach(files, new ParallelOptions {MaxDegreeOfParallelism = 50}, file => {
                var rpd = ProcessFile(file);

                if (rpd.RpdContent.Codes.Count == 0) {
                    Console.WriteLine("FAIL " + rpd.Name);
                }
						
                Console.WriteLine("SUCCESS " + rpd.Name + " " + string.Join(", ", rpd.RpdContent.Codes.Distinct()));
                
                queue.Enqueue(rpd);
            });

            return queue.ToList();
        }

        private Rpd ProcessFile(byte[] file, string extension) {
            var result = new Rpd();

            foreach (var extractor in _extractors.Where(t => t.IsSupport(extension))) {
                var extract = extractor.Extract(file, extension);
                
                result.RpdContent = _rpdContentExtractor.Extract(extract.Content);
                result.HasImageContent = extract.HasImageContent;
                
                return result;
            }

            result.IsValid = false;
            return result;
        }
    }
}
