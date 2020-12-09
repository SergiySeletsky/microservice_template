using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroService.Core
{
    public interface IServiceBus : ICoreService
    {
        void Publish<T>(T message) where T : ServiceBusMessage;

        void Subscribe<T>(Predicate<T> when, Action<T> action) where T : ServiceBusMessage;

        void Unsubscribe<T>(Action<T> action) where T : ServiceBusMessage;
    }
}
