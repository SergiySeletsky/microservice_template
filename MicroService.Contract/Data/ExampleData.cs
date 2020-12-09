using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MicroService.Contract.Data
{
    public class ExampleData
    {
        [DataMember(Order = 0)]
        public string Text { set; get; }
    }
}
