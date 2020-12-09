using MicroService.Core;
using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Registration;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web.Http;

namespace MicroService
{
    internal class ServiceEngine : IDisposable
    {
        private readonly INetworkService network;
        private readonly ILoggerService logger;
        private readonly CompositionContainer container;

        public ServiceEngine()
        {
            var builder = new RegistrationBuilder();

            builder.ForTypesDerivedFrom<ICoreService>().ExportInterfaces();

            builder.ForTypesMatching(t => t.IsClass && t.IsGenericType && t.Name.StartsWith("Repository"))
                .Export(e => e.AsContractType(typeof(IRepository<>)))
                .SetCreationPolicy(CreationPolicy.NonShared);

            var repoCtor = new Action<ParameterInfo, ImportBuilder>((info, importBuilder) =>
            {
                if (info.ParameterType.Name.StartsWith("IRepository"))
                {
                    importBuilder.AsMany(false);
                }
            });

            builder.ForTypesDerivedFrom<IService>()
                .SelectConstructor(c => c.SingleOrDefault(), repoCtor)
                .ExportInterfaces().Export();
            
            builder.ForTypesDerivedFrom<ApiController>()
                .SetCreationPolicy(CreationPolicy.NonShared)
                .Export().SelectConstructor(c => c.SingleOrDefault(), repoCtor);

            var cat = new ApplicationCatalog(builder);
            container = new CompositionContainer(cat, CompositionOptions.DisableSilentRejection | CompositionOptions.IsThreadSafe);
            container.ComposeExportedValue(container);
            container.ComposeParts();

            network = container.GetExportedValueOrDefault<INetworkService>();
            logger = container.GetExportedValueOrDefault<ILoggerService>();
        }

        public void Run()
        {
            foreach(var svc in container.GetExports<ICoreService>())
            {
                try
                {
                    if (!svc.IsValueCreated)
                    {
                        svc.Value.Initialize();
                        logger.Info("Core service initialized");
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("CoreService {0} is not initialized", svc);
                }
            }
            try
            {
                var svc = container.GetExportedValue<IService>();
                network.CreateServiceHost(svc);
                svc.Initialize(); 
            }
            catch(Exception ex)
            {
                Console.WriteLine("Main service is not initialized");
            }
        }

        public void Dispose()
        {
            foreach (var svc in container.GetExports<ICoreService>())
            {
                try
                {
                    if (svc.IsValueCreated)
                    {
                        svc.Value.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("CoreService {0} is not disposed", svc);
                }
            }
            try
            {
                var svc = container.GetExportedValue<IService>();
                svc.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Main service is not disposed");
            }
            container.Dispose();
        }
    }
}
