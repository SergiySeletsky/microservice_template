using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.Reflection;
using System.ServiceProcess;

namespace MicroService.ServiceInternals
{
    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            var serviceProcessInstaller = new ServiceProcessInstaller
            {
                Account = ServiceAccount.LocalSystem,
                Password = null,
                Username = null
            };

            var serviceInstaller = new ServiceInstaller
            {
                StartType = ServiceStartMode.Automatic
            };

            var asm = Assembly.GetEntryAssembly();
            serviceInstaller.ServiceName = asm.GetName().Name;

            var type = typeof(AssemblyDescriptionAttribute);
            if (Attribute.IsDefined(asm, type))
            {
                var attr = (AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(asm, type);
                serviceInstaller.DisplayName = attr.Description;
            }
            
            Installers.Add(serviceProcessInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
