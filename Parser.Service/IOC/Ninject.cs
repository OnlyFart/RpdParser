using System.Net;
using System.Text;
using Consul;
using Extractors.Configs;
using Extractors.ContentExtractors;
using Extractors.Contracts.ContentExtractors;
using Extractors.Contracts.DocumentExtractors;
using Extractors.Contracts.Types;
using Extractors.DocumentExtractors;
using FileGetter;
using FileGetter.Configs;
using JRPC.Registry.Ninject;
using JRPC.Service;
using JRPC.Service.Host.Kestrel;
using JRPC.Service.Registry;
using Microsoft.Extensions.Configuration;
using Ninject.Modules;
using Parser.Service.Configs;
using Parser.Service.Contracts.Logic;
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
            
            var dataExtractorConfig = config.GetSection("DataExtractor").Get<DataExtractorConfig>();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            ServicePointManager.DefaultConnectionLimit = 1000;
            
            Bind<IJrpcTransportService>().To<JRpcService>().InSingletonScope();
            Bind<IJrpcServicesRegistry>().To<JrpcServicesRegistry>();
            Bind<JRpcModule>().To<Service.ParserService>().InSingletonScope();
            Bind<IModulesRegistry>().To<NinjectModulesRegistry>();
            Bind<IJrpcServerHost>().To<KestrelJRpcServerHost>();
            Bind<IConsulClient>().To<ConsulClient>();


            Bind<IConfiguration>().ToConstant(config);
            Bind<IFileGetter>().To<FileNetworkGetter>().InSingletonScope();
            Bind<IProcessor>().To<Processor>().InSingletonScope();
            
            Bind<IYandexXmlProvider>().To<YandexXmlProvider>().InSingletonScope();

            Bind<RpdExtractorConfig>().ToConstant(dataExtractorConfig.RpdExtractor);

            Bind<IDocumentExtractor<DocumentBase>>().To<RpdContentExtractor>().InSingletonScope();

            Bind<IContentImageExtractor>().To<ContentImageExtractor>().InSingletonScope();
            Bind<ExtractorBase>().To<DocExtractor>().InSingletonScope();
            Bind<ExtractorBase>().To<PdfExtractor>().InSingletonScope();
        }
    }
}
