using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opaalib.Messaging
{
    public class OutboundMessageResponse
    {
        [JsonProperty(PropertyName = "resourceReference", Required = Required.Always)]
        public ResourceReference ResourceReference { get; set; }
    }

    public class ResourceReference
    {
        [JsonProperty(PropertyName = "resourceURL", Required = Required.Always)]
        public string ResourceUrl { get; set; }
    }
}
