using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroService.Core
{
    public interface ICoreService : IDisposable
    {
        void Initialize();
    }
}
