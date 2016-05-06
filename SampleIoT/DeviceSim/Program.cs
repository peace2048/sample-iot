using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;

namespace DeviceSim
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            // var host = new JobHost();
            // The following code ensures that the WebJob will be running continuously
            // host.RunAndBlock();

            var settings = Properties.Settings.Default;
            var client = DeviceClient.Create(settings.IoTHubUrl, new DeviceAuthenticationWithRegistrySymmetricKey(settings.DeviceId, settings.AuthKey));
            var rand = new Random();
            while (true)
            {
                var s = JsonConvert.SerializeObject(new { Id = 1, Time = DateTime.Now, Value = rand.NextDouble() * 10 + 15 });
                client.SendEventAsync(new Message(Encoding.UTF8.GetBytes(s))).Wait();
                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));
            }
        }
    }
}
