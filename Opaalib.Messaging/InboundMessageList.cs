using Newtonsoft.Json;
using System;

namespace Opaalib.Messaging
{
    public class InboundMessageListContainer
    {
        [JsonProperty(PropertyName = "inboundMessageList", Required = Required.Always)]
        public InboundMessageList InboundMessageList { get; set; }
    }

    public class InboundMessageList
    {
        [JsonProperty(PropertyName = "inboundMessage")]
        public InboundMessage[] InboundMessage { get; set; }

        /// <summary>
        /// The total number of messages in the gateway
        /// storage waiting for retrieval at the time of the
        /// request.
        /// </summary>
        [JsonProperty(PropertyName = "totalNumberOfPendingMessages")]
        public int TotalNumberOfPendingMessages { get; set; }

        /// <summary>
        /// The number of messages included in the
        /// response.
        /// </summary>
        [JsonProperty(PropertyName = "numberOfMessagesInThisBatch")]
        public int NumberOfMessagesInThisBatch { get; set; }

        /// <summary>
        /// Self-referring URL.  
        /// </summary>
        [JsonProperty(PropertyName = "resourceURL", Required = Required.Always)]
        public string ResourceUrl { get; set; }
    }

    public class InboundMessage
    {
        /// <summary>
        /// The destination address used by the terminal to
        /// send the MO message.
        /// </summary>
        [JsonProperty(PropertyName = "destinationAddress", Required = Required.Always)]
        public string DestinationAddress { get; set; }

        /// <summary>
        /// The address of the sender of the MO message.  
        /// </summary>
        [JsonProperty(PropertyName = "senderAddress", Required = Required.Always)]
        public string SenderAddress { get; set; }

        /// <summary>
        /// The date and time when the message is received
        /// by the operator.
        /// </summary>
        [JsonProperty(PropertyName = "dateTime")]
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Self-referring URL. Not present, as the message is
        /// deleted.
        /// </summary>
        [JsonProperty(PropertyName = "resourceURL")]
        public string ResourceUrl { get; set; }

        /// <summary>
        /// Server generated message identifier.  
        /// </summary>
        [JsonProperty(PropertyName = "messageId")]
        public string MessageId { get; set; }

        /// <summary>
        /// The SMS text message. 
        /// </summary>
        [JsonProperty(PropertyName = "inboundSMSTextMessage", Required = Required.Always)]
        public InboundSmsTextMessage InboundSmsTextMessage { get; set; }
    }

    // This is probably same as OutboundSmsTextMessage
    public class InboundSmsTextMessage
    {
        [JsonProperty(PropertyName = "message", Required = Required.Always)]
        public string Message { get; set; }
    }
}