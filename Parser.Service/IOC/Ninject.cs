using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using Consul;
using Extractors.ContentExtractors;
using Extractors.ContentExtractors.ContentImageExtractors;
using FileGetter;
using GemBox.Pdf;
using JRPC.Registry.Ninject;
using JRPC.Service;
using JRPC.Service.Host.Kestrel;
using JRPC.Service.Registry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Ninject.Modules;
using Parser.Service.Configs;
using Parser.Service.Logic;
using Yandex.Xml;
using Yandex.Xml.Configs;
using Yandex.Xml.Contracts;

namespace Parser.Service.IOC {
    public class ParserServiceNinjectModule : NinjectModule {
        public override void Load() {
            // Пакет Gembox для работы с PDF платный.
            // Что бы использовать пробную версию надо прописывать такой ключ
            // В пробной версии можно обрабатывать только несколько первых страниц
            // Что для текущей задачи в самый раз
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");

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
