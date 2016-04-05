using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.Devices.Client;

namespace DeviceSim
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            //var host = new JobHost();
            // The following code ensures that the WebJob will be running continuously
            //host.RunAndBlock();

            var settings = Properties.Settings.Default;
            var client = DeviceClient.Create(settings.IoTHubUrl, new DeviceAuthenticationWithRegistrySymmetricKey(settings.DeviceId, settings.AuthKey));
            client.SendEventAsync(new Message(Encoding.UTF8.GetBytes("test!!"))).Wait();
        }
    }
}
