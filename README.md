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

// poll for incoming messages
var inboundMessages = await messenger.RetrieveAndDeleteMessagesAsync(new InboundMessageRetrieveAndDeleteRequest
{
    UseAttachmentUrls = true,
}, "my registration id");

// wait for notifications
var inboundObs = Observer.Create<InboundMessageNotification>(v => { /* handle inbound message here */ });
var outboundStatusObs = Observer.Create<DeliveryInfoNotification>(v => { /* handle outbound status here */ });

using (messenger.StartReceivingNotifications(
    new Uri("http://192.168.1.10:80/inbound/"), inboundObs, // inbound message http server config
    new Uri("http://192.168.1.10:80/outboundstatus/"), outboundStatusObs)) // outbound message status http server config
{

}
```

## Requirements

* .NET Framework 4.0 or higher
* Visual Studio 2017 (for compiling)

## Implementation status

- [x] OAuth
- [x] Messaging
- [ ] Payment