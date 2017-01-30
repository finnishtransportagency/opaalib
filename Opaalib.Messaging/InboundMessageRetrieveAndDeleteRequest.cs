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
        // TODO: What are the valid values
        /// <summary>
        /// Specifies the order in which to retrieve
        /// messages, if there are more than one pending.
        /// Valid value: OldestFirst
        /// </summary>
        [JsonProperty(PropertyName = "retrievalOrder")]
        public string RetrievalOrder { get; set; }

        // TODO: use enum instead?
        /// <summary>
        /// The priority of the message. Defaults to Normal.
        /// </summary>
        [JsonProperty(PropertyName = "priority")]
        public string Priority { get; set; }

        /// <summary>
        /// The maximum number of messages to return in
        /// the response.
        /// </summary>
        [JsonProperty(PropertyName = "maxBatchSize")]
        public int MaxBatchSize { get; set; }

        /// <summary>
        /// If set to ‘true’, inbound messages have links to
        /// attachments together with the indication of the
        /// content type, and optionally the size of each 
        /// attachment.<para />
        /// Otherwise, inbound messages include
        /// attachments using MIME.
        /// </summary>
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
