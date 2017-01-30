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
        /// <summary>
        /// Self-referring URL - a link to the created message resource.
        /// Includes the requestId.Can be used by application to check
        /// the status of message delivery.
        /// </summary>
        [JsonProperty(PropertyName = "resourceURL", Required = Required.Always)]
        public string ResourceUrl { get; set; }
    }
}
