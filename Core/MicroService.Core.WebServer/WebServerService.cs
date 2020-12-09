using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace MicroService.Core.WebServer
{
    public class WebServerService : IWebServerService
    {
        private readonly HttpSelfHostServer server;

        public WebServerService(IConfigService cfgsvc, CompositionContainer container)
        {
            if (!cfgsvc.Exists(Cfg.WebServerPort)) cfgsvc.Set(Cfg.WebServerPort, 8080);

            var config = new HttpSelfHostConfiguration("http://0.0.0.0:" + cfgsvc.Get<int>(Cfg.WebServerPort))
            {
                DependencyResolver = new DependencyResolver(container)
            };

            container.ComposeExportedValue((HttpConfiguration)config);

            config.MapHttpAttributeRoutes();

            config.EnableCors();

            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());

            server = new HttpSelfHostServer(config);
        }

        public void Initialize()
        {
            try
            {
                server.OpenAsync().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async void Dispose()
        {
            await server.CloseAsync();
        }  
    }
}
