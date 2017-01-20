using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Opaalib.Shared
{
    public class MyWebClient : WebClient
    {
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10d);

        protected override WebRequest GetWebRequest(Uri address)
        {
            var baseRequest = base.GetWebRequest(address);
            baseRequest.Timeout = (int)Timeout.TotalMilliseconds;
            return baseRequest;
        }
    }
}
