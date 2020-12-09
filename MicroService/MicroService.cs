using System;
using System.ServiceProcess;
using MicroService.ServiceInternals;
using System.Net;
using System.Diagnostics;

namespace MicroService
{
    public class MicroService
    {
        public event Action OnServiceStarted;
        public event Action OnServiceStopped;

        private readonly int port;
        private readonly WindowsServiceManager serviceManager;
        private readonly ServiceEngine engine = new ServiceEngine(); 

        public MicroService(int port = 8080)
        {
            this.port = port;

            serviceManager = new WindowsServiceManager();

            InternalService.OsStarted += Start;
            InternalService.OsStopped += Stop;
        }

        public void Run(string[] args)
        {
            if (args.Length == 0)
            {
                RunConsole();
                return;
            }

            switch (args[0])
            {
                case "-service":
                    RunService();
                    break;
                case "-install":
                    InstallService();
                    break;
                case "-uninstall":
                    UnInstallService();
                    break;
                default:
                    throw new Exception(args[0] + " is not a valid command!");
            }
        }

        private void RunConsole()
        {
            Start();
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
            Stop();
        }

        private static void RunService()
        {
            ServiceBase[] servicesToRun = {new InternalService()};
            ServiceBase.Run(servicesToRun);
        }

        private void InstallService()
        {
            serviceManager.Install();
            serviceManager.Start();
        }

        private void UnInstallService()
        {
            serviceManager.Stop();
            serviceManager.UnInstall();
        }

        private void Stop()
        {
            engine.Dispose();
            if ( OnServiceStopped != null)
                OnServiceStopped.Invoke();
        }

        private void Start()
        {
            engine.Run();
            Console.WriteLine("Service started on port {0}", port);
            if ( OnServiceStarted != null)
                OnServiceStarted.Invoke();
        }
    }
}
