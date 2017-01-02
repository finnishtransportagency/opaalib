using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opaalib.Messaging
{
    public class InboundMessageRetrieveAndDeleteRequestContainer
    {
        [JsonProperty(PropertyName = "inboundMessageRetrieveAndDeleteRequest")]
        public InboundMessageRetrieveAndDeleteRequest InboundMessageRetrieveAndDeleteRequest { get; set; }
    }

    public class InboundMessageRetrieveAndDeleteRequest
    {
        // TODO: use enum instead?
        [JsonProperty(PropertyName = "retrievalOrder")]
        public string RetrievalOrder { get; set; }

        // TODO: use enum instead?
        [JsonProperty(PropertyName = "priority")]
        public string Priority { get; set; }

        [JsonProperty(PropertyName = "maxBatchSize")]
        public int MaxBatchSize { get; set; }

        [JsonProperty(PropertyName = "useAttachmentURLs", Required = Required.Always)]
        public bool UseAttachmentUrls { get; set; }
    }

    // TODO: What does this data type contain?
    public class MessagePriority
    {

    }

    // TODO: What does this data type contain?
    public class RetrievalOrder
    {

    }
}
