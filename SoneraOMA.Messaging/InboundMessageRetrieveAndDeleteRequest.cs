using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoneraOMA.Messaging
{
    public class InboundMessageRetrieveAndDeleteRequestContainer
    {
        [JsonProperty(PropertyName = "inboundMessageRetrieveAndDeleteRequest")]
        public InboundMessageRetrieveAndDeleteRequest InboundMessageRetrieveAndDeleteRequest { get; set; }
    }

    public class InboundMessageRetrieveAndDeleteRequest
    {
        [JsonProperty(PropertyName = "retrievalOrder")]
        public RetrievalOrder RetrievalOrder { get; set; }

        [JsonProperty(PropertyName = "priority")]
        public MessagePriority Priority { get; set; }

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
