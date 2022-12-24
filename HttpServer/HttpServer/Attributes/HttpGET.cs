using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer.Attributes
{
    public class HttpGET : HttpMethod
    {
        public HttpGET(string methodUri) : base(methodUri) { }
        public HttpGET() : base() { }
    }
}
