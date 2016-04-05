using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscribeIoTHub
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = Properties.Settings.Default;
            var hubClient = EventHubClient.CreateFromConnectionString(settings.EventHubConnectionString, "messages/events");
            var cancellationTokenSource = new System.Threading.CancellationTokenSource();
            foreach (var partitionId in hubClient.GetRuntimeInformation().PartitionIds)
            {
                Subscribe(hubClient, partitionId, cancellationTokenSource.Token).ContinueWith(t => { });
            }
            Console.WriteLine("Started. hit any key to exit.");
            Console.ReadLine();
            cancellationTokenSource.Cancel();
        }

        static async Task Subscribe(Microsoft.ServiceBus.Messaging.EventHubClient client, string partitionId, System.Threading.CancellationToken cancellationToken)
        {
            var reciever = await client.GetDefaultConsumerGroup().CreateReceiverAsync(partitionId, DateTime.UtcNow);
            while (!cancellationToken.IsCancellationRequested)
            {
                var eventData = await reciever.ReceiveAsync();
                if (eventData == null) continue;

                var text = Encoding.UTF8.GetString(eventData.GetBytes());
                Console.WriteLine($"Message received. Partition: {partitionId} Data: {text}");
            }
        }
    }
}
