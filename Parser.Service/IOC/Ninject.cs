using System.Net;
using System.Text;
using Consul;
using Extractors.ContentExtractors;
using Extractors.ContentExtractors.ContentImageExtractors;
using FileGetter;
using JRPC.Registry.Ninject;
using JRPC.Service;
using JRPC.Service.Host.Kestrel;
using JRPC.Service.Registry;
using Microsoft.Extensions.Configuration;
using Ninject.Modules;
using Parser.Service.Configs;
using Parser.Service.Logic;
using Yandex.Xml;
using Yandex.Xml.Configs;
using Yandex.Xml.Contracts;

namespace Parser.Service.IOC {
    public class ParserServiceNinjectModule : NinjectModule {
        public override void Load() {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            
            var xmlConfig = config.GetSection("YandexXml").Get<YandexXmlConfig>();
            var processorConfig = config.GetSection("Processor").Get<ProcessorConfig>();
            
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            ServicePointManager.DefaultConnectionLimit = 1000;
            
            Bind<IJrpcTransportService>().To<JRpcService>().InSingletonScope();
            Bind<IJrpcServicesRegistry>().To<JrpcServicesRegistry>();
            Bind<JRpcModule>().To<Service.ParserService>().InSingletonScope();
            Bind<IModulesRegistry>().To<NinjectModulesRegistry>();
            Bind<IJrpcServerHost>().To<KestrelJRpcServerHost>();
            Bind<IConsulClient>().To<ConsulClient>();
            Bind<IFileGetter>().To<FileNetworkGetter>().InSingletonScope();
            
            Bind<ProcessorConfig>().ToConstant(processorConfig);
            Bind<IProcessor>().To<Processor>().InSingletonScope();
            
            Bind<YandexXmlConfig>().ToConstant(xmlConfig);
            Bind<IYandexXml>().To<YandexXml>().InSingletonScope();
            
            Bind<IContentImageExtractor>().To<ContentImageExtractor>().InSingletonScope();
            Bind<ExtractorBase>().To<DocExtractor>().InSingletonScope();
            Bind<ExtractorBase>().To<PdfExtractor>().InSingletonScope();
        }
    }
}
