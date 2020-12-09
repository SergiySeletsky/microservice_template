using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace MicroService.Core.Network
{
    internal class ErrorHandler : IErrorHandler
    {
        public bool HandleError(Exception ex)
        {
            return true;
        }

        public void ProvideFault(Exception ex, MessageVersion version, ref Message msg)
        {
            if (msg == null) throw new ArgumentNullException("msg");
            var newEx = new FaultException(string.Format("CALLED FROM {0}", ex.TargetSite.Name));
            var msgFault = newEx.CreateMessageFault();
            msg = Message.CreateMessage(version, msgFault, newEx.Action);
        }
    }
}
