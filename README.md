# Opaalib

#### Send and receive messages using Sonera-OMA Messaging API

## Example

```c#
// initialize
var authenticator = new Authenticator("username", "password");
var messenger = new Messenger(authenticator);

var senderAddress = "tel:+3587654321";
var destinationAddress = "tel:+3581234567";

// send an outbound message
var outboundResponse = await messenger.OutboundMessageRequestAsync(new OutboundMessageRequest
{
    Address = new List<string> { destinationAddress },
    SenderAddress = senderAddress,
    OutboundSmsTextMessage = new OutboundSmsTextMessage
    {
        Message = "example message",
    },
});

// check outbound message status
var outboundStatus = await messenger.ReadOutboundMessageDeliveryStatusAsync(outboundResponse.ResourceReference.ResourceUrl);

// check for incoming messages
var inboundMessages = await messenger.RetrieveAndDeleteMessagesAsync(new InboundMessageRetrieveAndDeleteRequest
{
    UseAttachmentUrls = true,
}, "my registration id");
```

## Requirements

* .NET Framework 4.0 or higher

## Implementation status

- [x] OAuth
- [x] Messaging
- [ ] Payment