using Newtonsoft.Json;
using SoneraOMA.OAuth;
using SoneraOMA.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoneraOMA.Example
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
