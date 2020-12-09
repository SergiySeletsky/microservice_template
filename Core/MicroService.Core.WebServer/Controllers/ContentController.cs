using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace MicroService.Core.WebServer.Controllers
{
    [EnableCors("*", "*", "*")]
    [RoutePrefix("")]
    public class ContentController : ApiController
    {
        [HttpGet, Route("{path=index.html}")]
        public HttpResponseMessage View(string path)
        {
            return Get(path, "text/html");
        }

        [HttpGet, Route("css/{path}")]
        public HttpResponseMessage Css(string path)
        {
            return Get(path, "text/css");
        }

        [HttpGet, Route("js/{path}")]
        public HttpResponseMessage Js(string path)
        {
            return Get(path, "text/javascript");
        }

        [HttpGet, Route("img/{path}")]
        public HttpResponseMessage Img(string path)
        {
            return Get(path, null);
        }

        private HttpResponseMessage Get(string path, string mime, [CallerMemberName]string folder = "")
        {
            var msg = new HttpResponseMessage();
            var asm = Assembly.GetEntryAssembly();
            using (var stream = asm.GetManifestResourceStream($"{asm.GetName().Name}.www.{folder.ToLower()}.{path}"))
            {
                if (stream != null)
                {
                    if (mime == null)
                    {
                        using (var ms = new MemoryStream())
                        {
                            stream.CopyTo(ms);
                            msg.Content = new ByteArrayContent(ms.ToArray());
                        }
                    }
                    else
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            msg.Content = new StringContent(reader.ReadToEnd(), Encoding.UTF8, mime);
                        }
                    }
                }
                else
                {
                    msg.StatusCode = HttpStatusCode.NotFound;
                    msg.Content = new StringContent("404 - Content not found", Encoding.UTF8, mime);
                }
            }
            return msg;
        }
    }
}
