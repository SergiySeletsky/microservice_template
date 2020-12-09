using MicroService.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using MicroService.Contract.CallBack;
using MicroService.Contract.Data;
using MicroService.Contract.Service;

namespace MicroService.Example
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Single, 
        InstanceContextMode = InstanceContextMode.Single, 
        IncludeExceptionDetailInFaults = true)]
    public class CallExample : IExampleCallback
    {
        public void Ping()
        {
            Console.WriteLine("callback called");
        }
    }

    [ServiceBehavior(
        ConcurrencyMode = ConcurrencyMode.Multiple,
        InstanceContextMode = InstanceContextMode.Single,
        AddressFilterMode = AddressFilterMode.Any,
        IncludeExceptionDetailInFaults = true)]
    public class TestNetService : ITestNetService
    {
        private readonly INetworkService netSvc;
        private readonly IConfigService cfgsvc;
        private readonly IStorageService store;
        private readonly IRepository<object> repo;

        public TestNetService(INetworkService netSvc, IConfigService cgfSvc, IStorageService storeSvc, IRepository<object> repo)
        {
            this.netSvc = netSvc;
            this.cfgsvc = cgfSvc;
            this.store = storeSvc;
            this.repo = repo;
            //IServiceBus bus = null;
            //bus.Subscribe<NewUser>(when => when.Target == Module.Name, msg => {
            //    Console.WriteLine("New user {0} registred on {1} server.", msg.UserName, msg.Source);
            //});

            //bus.Subscribe<Upgrade>(when => when.Target == Module.Name && when.Version > Module.Version, msg => {
            //    Console.WriteLine("New update {0} avialible, restart is needed : {1}.", msg.Version, msg.RestartRequired);
            //});
        }

        public void Initialize()
        {
            var res = netSvc.Discovery<ITestNetService>(50);

            netSvc.Use<ITestNetService>(t =>
            {
                t.SendData(new ExampleData() { Text = "ha ha ha" });
            }, new CallExample(), res.Endpoints[0].Address.Uri);

            var sw = Stopwatch.StartNew();
            var web = new WebClient();
            for (var i = 0; i < 10000; i++)
            {
                var text = web.DownloadString("http://localhost:8080/");
                if (text.Length != 186)
                {
                    throw new Exception();
                }
            }
            sw.Stop();
            Console.WriteLine("Time to load: {0}", sw.ElapsedMilliseconds);
        }

        public void Dispose()
        {
            repo.Dispose();
        }

        [OperationBehavior]
        public ExampleData SendData(ExampleData data)
        {
            Console.WriteLine("from client: {0}", data.Text);
            var callback = OperationContext.Current.GetCallbackChannel<IExampleCallback>();
            callback.Ping();
            return new ExampleData() { Text = "this is text from service" };
        }
    }
}