using Newtonsoft.Json;

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

        [JsonProperty(PropertyName = "totalNumberOfPendingMessages")]
        public int TotalNumberOfPendingMessages { get; set; }

        [JsonProperty(PropertyName = "numberOfMessagesInThisBatch")]
        public int NumberOfMessagesInThisBatch { get; set; }

        // TODO: What is anyURI type?
        [JsonProperty(PropertyName = "resourceURL", Required = Required.Always)]
        public string ResourceUrl { get; set; }
    }

    public class InboundMessage
    {
        [JsonProperty(PropertyName = "destinationAddress", Required = Required.Always)]
        public string DestinationAddress { get; set; }

        [JsonProperty(PropertyName = "senderAddress", Required = Required.Always)]
        public string SenderAddress { get; set; }

        // TODO: Add converter (what is the date time format?)
        [JsonProperty(PropertyName = "dateTime")]
        public string DateTime { get; set; }

        [JsonProperty(PropertyName = "resourceURL")]
        public string ResourceUrl { get; set; }

        [JsonProperty(PropertyName = "messageId")]
        public string MessageId { get; set; }

        [JsonProperty(PropertyName = "inboundSMSTextMessage", Required = Required.Always)]
        public InboundSmsTextMessage InboundSmsTextMessage { get; set; }
    }

    // TODO: What does this datatype contain? Is this same as outbound version?
    public class InboundSmsTextMessage
    {
        [JsonProperty(PropertyName = "message", Required = Required.Always)]
        public string Message { get; set; }
    }
}