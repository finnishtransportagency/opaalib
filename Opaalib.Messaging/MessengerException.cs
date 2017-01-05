using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Opaalib.Messaging
{
    public class MessengerException : Exception
    {
        public RequestError RequestError { get; }

        public MessengerException(string message, Exception innerException)
            : base(message, innerException)
        {
            if (!(innerException is WebException we)) return;

            try
            {
                var response = (HttpWebResponse)we.Response;
                var statusCode = response.StatusCode;

                using (var stream = response.GetResponseStream())
                using (var sr = new StreamReader(stream))
                {
                    var responseJson = sr.ReadToEnd();
                    RequestError = JsonConvert.DeserializeObject<RequestErrorContainer>(responseJson).RequestError;
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
