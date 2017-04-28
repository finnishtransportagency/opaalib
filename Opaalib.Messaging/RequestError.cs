using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opaalib.Messaging
{
    public class RequestErrorContainer
    {
        [JsonProperty(PropertyName = "requestError", Required = Required.Always)]
        public RequestError RequestError { get; set; }
    }

    public class RequestError
    {
        [JsonProperty(PropertyName = "serviceException")]
        public ServiceException ServiceException { get; set; }

        [JsonProperty(PropertyName = "policyException")]
        public PolicyException PolicyException { get; set; }
    }

    public class ServiceException
    {
        [JsonProperty(PropertyName = "messageId", Required = Required.Always)]
        public string MessageId { get; set; }

        [JsonProperty(PropertyName = "text", Required = Required.Always)]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "variables", Required = Required.Always)]
        public string[] Variables { get; set; }

        public string FormattedText
        {
            get
            {
                var text = Text;

                for (int i = 0; i < Variables.Length; i++)
                {
                    text = text.Replace($"%{i + 1}", Variables[i]);
                }

                return text;
            }
        }
    }

    // HACK: Duplicate class :|
    public class PolicyException
    {
        [JsonProperty(PropertyName = "messageId", Required = Required.Always)]
        public string MessageId { get; set; }

        [JsonProperty(PropertyName = "text", Required = Required.Always)]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "variables", Required = Required.Always)]
        public string[] Variables { get; set; }

        public string FormattedText
        {
            get
            {
                var text = Text;

                for (int i = 0; i < Variables.Length; i++)
                {
                    text = text.Replace($"%{i + 1}", Variables[i]);
                }

                return text;
            }
        }
    }
}
