namespace Parser.Service.Configs {
    public class ProcessorConfig {
        public int MaxParallelThreads { get; set; }
        public string BaseDirectory { get; set; }
        public string[] XmlPatterns { get; set; }
    }
}
