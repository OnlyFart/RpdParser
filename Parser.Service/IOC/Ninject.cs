using Consul;
using Extractors.ContentExtractors;
using Extractors.ContentExtractors.ContentImageExtractors;
using GemBox.Pdf;
using JRPC.Registry.Ninject;
using JRPC.Service;
using JRPC.Service.Host.Kestrel;
using JRPC.Service.Registry;
using Ninject.Modules;

namespace Parser.Service.IOC {
    public class ParserServiceNinjectModule : NinjectModule {
        public override void Load() {
            // Пакет Gembox для работы с PDF платный.
            // Что бы использовать пробную версию надо прописывать такой ключ
            // В пробной версии можно обрабатывать только несколько первых страниц
            // Что для текущей задачи в самый раз
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
            
            Bind<IJrpcTransportService>().To<JRpcService>().InSingletonScope();
            Bind<IJrpcServicesRegistry>().To<JrpcServicesRegistry>();
            Bind<JRpcModule>().To<Service.ParserService>().InSingletonScope();
            Bind<IModulesRegistry>().To<NinjectModulesRegistry>();
            Bind<IJrpcServerHost>().To<KestrelJRpcServerHost>();
            Bind<IConsulClient>().To<ConsulClient>();
            Bind<IContentImageExtractor>().To<ContentImageExtractor>().InSingletonScope();
            Bind<ExtractorBase>().To<PdfExtractor>().InSingletonScope();
            Bind<ExtractorBase>().To<DocExtractor>().InSingletonScope();
        }
    }
}
