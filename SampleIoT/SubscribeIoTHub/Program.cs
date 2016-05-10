using Microsoft.AspNet.SignalR.Client;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypeDefinition;
using System.Runtime.Serialization.Json;

namespace SubscribeIoTHub
{
    class Program
    {
        private static IHubProxy _signalr;
        private static object _lock = new object();

        static void Main(string[] args)
        {
            var settings = Properties.Settings.Default;
            var hubConn = new HubConnection(settings.SignalRHubUrl);
            _signalr = hubConn.CreateHubProxy(settings.SignalRHubName);
            hubConn.Start().Wait();

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
            try
            {
                var reciever = await client.GetDefaultConsumerGroup().CreateReceiverAsync(partitionId, DateTime.UtcNow);
                while (!cancellationToken.IsCancellationRequested)
                {
                    var eventData = await reciever.ReceiveAsync();
                    if (eventData == null) continue;

                    var text = Encoding.UTF8.GetString(eventData.GetBytes());
                    Console.WriteLine($"Message received. Partition: {partitionId} Data: {text}");

                    ////var json = Newtonsoft.Json.Linq.JObject.Parse(text);
                    var json = Newtonsoft.Json.JsonConvert.DeserializeObject<upContainer>(text);

                    lock (_lock)
                    {
                        //var id = (int)json["node_id"];
                        //var time = (DateTime)json["Time"];
                        //var value = (double)json["temperature"];
                        //_signalr.Invoke("SendValue", id, time, value).Wait();

                        if( json.data != null)
                        {
                            var id = 1;
                            var sensor = json.data.Where(t => t.node_id == id).FirstOrDefault();
                            _signalr.Invoke("SendValue", sensor.node_id, sensor.Time, sensor.temperature).Wait();
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
