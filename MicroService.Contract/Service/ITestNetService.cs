using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using MicroService.Contract.CallBack;
using MicroService.Contract.Data;
using MicroService.Core;

namespace MicroService.Contract.Service
{
    [ServiceContract(CallbackContract = typeof(IExampleCallback), SessionMode = SessionMode.Required)]
    public interface ITestNetService : IService
    {
        [OperationContract]
        ExampleData SendData(ExampleData data);
    }
}
