using Newtonsoft.Json;
using Opaalib.OAuth;
using Opaalib.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opaalib.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            var oauth = new Authenticator("test", "asd", AuthenticatorConfiguration.Default);
            var accessToken = oauth.RequestAccessTokenAsync().Result;
        }
    }
}
