using Opaalib.OAuth;
using Opaalib.Messaging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reactive;

namespace Opaalib.Example
{
    class Program
    {
        static async Task AsyncMain()
        {
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
        }

        static void Main(string[] args)
        {
            AsyncMain().Wait();
            Console.ReadKey();
        }
    }
}
