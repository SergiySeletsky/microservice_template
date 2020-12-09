using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroService.Core
{
    public interface ILoggerService : ICoreService
    {
        void Debug(string message, params object[] @params);

        void Error(Exception exception, string message, params object[] @params);

        void Info(string message, params object[] @params);

        void Warn(string message, params object[] @params);
    }
}
