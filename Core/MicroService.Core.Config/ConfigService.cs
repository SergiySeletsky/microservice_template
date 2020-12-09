using System;
using System.Configuration;
using System.Linq;

namespace MicroService.Core.Config
{
    internal class ConfigService : IConfigService
    {
        private readonly Configuration config;

        public ConfigService()
        {
            config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        }

        public void Initialize()
        {
            //if(!Exists(Cfg.WebServerPort)) Set(Cfg.WebServerPort, 9019);
        }

        public bool Exists(Enum cfg)
        {
            return config.AppSettings.Settings.AllKeys.Contains(cfg.ToString());
        }

        public void Set(Enum cfg, object value)
        {
            if (Exists(cfg))
            {
                config.AppSettings.Settings[cfg.ToString()].Value = value.ToString();
            }
            else
            {
                config.AppSettings.Settings.Add(cfg.ToString(), value.ToString());
            }
            config.Save(ConfigurationSaveMode.Modified);
        }

        public T Get<T>(Enum cfg)
        {
            if (config.AppSettings.Settings.AllKeys.Contains(cfg.ToString()))
            {
                var val = config.AppSettings.Settings[cfg.ToString()].Value;
                if (typeof(T) == typeof(TimeSpan))
                {
                    return (T)(object)TimeSpan.Parse(val);
                }
                return (T)Convert.ChangeType(val, typeof(T));
            }
            return default(T);
        }

        public void Dispose()
        {
            config.Save(ConfigurationSaveMode.Modified);
        }
    }
}
