using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroService.Core
{
    public interface IConfigService : ICoreService
    {
        void Set(Enum cfg, object value);

        T Get<T>(Enum cfg);

        bool Exists(Enum cfg);
    }
}
