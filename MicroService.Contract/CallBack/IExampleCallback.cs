using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;

namespace MicroService.Contract.CallBack
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface IExampleCallback
    {
        [OperationContract(IsOneWay = true)]
        void Ping();
    }
}
