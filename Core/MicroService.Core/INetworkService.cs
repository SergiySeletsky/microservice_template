using System;
using System.ServiceModel.Discovery;

namespace MicroService.Core
{
    public interface INetworkService : ICoreService
    {
        void CreateServiceHost(IService svc);
        void Use<T>(Action<T> action, object handler, Uri uri);
        FindResponse Discovery<T>(int timeout) where T : IService;
    }
}
