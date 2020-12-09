using Microsoft.Win32;
using System.Reflection;

namespace MicroService
{
    internal class RegistryManipulator
    {
        private readonly string serviceRegistryPath;
        private const string Imagepath = "ImagePath";
        private const string MinusService = " -service";

        public RegistryManipulator()
        {
            var asm = Assembly.GetEntryAssembly();
            serviceRegistryPath = @"SYSTEM\CurrentControlSet\services\" + asm.GetName().Name;
        }

        internal void RemoveServiceFromRegistry()
        {
            var key = Registry.LocalMachine.OpenSubKey(serviceRegistryPath, true);
            var path = key.GetValue(Imagepath).ToString().Replace(MinusService, "");
            key.SetValue(Imagepath,path);
            key.Close();
        }

        internal void AddServiceToRegistry()
        {
            var key = Registry.LocalMachine.OpenSubKey(serviceRegistryPath, true);
            var path = key.GetValue(Imagepath) + MinusService;
            key.SetValue(Imagepath,path);
            key.Close();
        }
    }
}
