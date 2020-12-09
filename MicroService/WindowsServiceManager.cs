using System;
using System.Collections;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;

namespace MicroService
{
    internal class WindowsServiceManager
    {
        private readonly string serviceName;
        private readonly RegistryManipulator registryManipulator;

        public WindowsServiceManager()
        {
            var asm = Assembly.GetEntryAssembly();
            this.serviceName = asm.GetName().Name;
            registryManipulator = new RegistryManipulator();
        }

        internal void Install()
        {
            if (IsInstalled()) return;
            using (var installer = GetInstaller())
            {
                IDictionary state = new Hashtable();
                try
                {
                    installer.Install(state);
                    installer.Commit(state);
                    registryManipulator.AddServiceToRegistry();
                }
                catch
                {
                    try
                    {
                        installer.Rollback(state);
                    }
                    catch { }
                    throw;
                }
            }
        }

        internal void UnInstall()
        {
            if (!IsInstalled()) return;
            using (var installer = GetInstaller())
            {
                IDictionary state = new Hashtable();
                installer.Uninstall(state);
            }
            registryManipulator.RemoveServiceFromRegistry();
        }

        internal void Start()
        {
            if (!IsInstalled()) return;
            using (var controller = new ServiceController(serviceName))
            {
                if (controller.Status == ServiceControllerStatus.Running) return;
                controller.Start();
                controller.WaitForStatus(ServiceControllerStatus.Running,TimeSpan.FromSeconds(10));
            }
        }

        internal void Stop()
        {
            if (!IsInstalled()) return;
            using (var controller = new ServiceController(serviceName))
            {
                if (controller.Status == ServiceControllerStatus.Stopped) return;
                controller.Stop();
                controller.WaitForStatus(ServiceControllerStatus.Stopped,TimeSpan.FromSeconds(10));
            }
        }

        private static AssemblyInstaller GetInstaller()
        {
            var serviceExeName = Assembly.GetEntryAssembly().ManifestModule.Name;
            return new AssemblyInstaller(serviceExeName,null){ UseNewContext = true};
        }

        private bool IsInstalled()
        {
            using (var controller = new ServiceController(serviceName))
            {
                try
                {
                    var status = controller.Status;
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
