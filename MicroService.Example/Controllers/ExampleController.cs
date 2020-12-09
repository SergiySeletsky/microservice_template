using System;
using System.Web.Http;
using System.Web.Http.Cors;
using MicroService.Contract.Data;
using MicroService.Contract.Service;
using MicroService.Core;

namespace MicroService.Example.Controllers
{
    [RoutePrefix("api/Example"), EnableCors("*","*","*")]
    public class ExampleController : ApiController
    {
        private readonly INetworkService netsvc;
        public ExampleController(INetworkService netsvc)
        {
            this.netsvc = netsvc;
        }

        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok("Example");
        }

        [Route("{id}")]
        public IHttpActionResult Get(int id)
        {
            var res = netsvc.Discovery<ITestNetService>(50);

            netsvc.Use<ITestNetService>(t =>
            {
                t.SendData(new ExampleData() { Text = id.ToString() });
            }, new CallExample(), res.Endpoints[0].Address.Uri);

            return Ok("Example" + id);
        }
    }
}
