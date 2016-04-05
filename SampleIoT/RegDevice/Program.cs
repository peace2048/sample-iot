using Microsoft.Azure.Devices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegDevice
{
    class Program
    {
        static void Main(string[] args)
        {
            var registryManager = RegistryManager.CreateFromConnectionString(Properties.Settings.Default.IoTHubConnectionString);
            if (args.Length == 1 && args[0].ToLower() == "list")
            {
                return;
            }
            if (args.Length > 1 && args[0].ToLower() == "add")
            {
                registryManager.OpenAsync();
                try
                {
                    foreach (var deviceId in args.Skip(1))
                    {
                        try
                        {
                            var device = registryManager.AddDeviceAsync(new Device(deviceId)).Result;
                            Console.WriteLine($"Generated device [{device.Id}] access key: {device.Authentication.SymmetricKey.PrimaryKey}");
                        }
                        catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceAlreadyExistsException)
                        {
                            var device = registryManager.GetDeviceAsync(deviceId).Result;
                            Console.WriteLine($"Already generated device [{device.Id}] access key: {device.Authentication.SymmetricKey.PrimaryKey}");
                        }
                        catch (System.AggregateException e)
                        {
                            if (e.InnerException is Microsoft.Azure.Devices.Common.Exceptions.DeviceAlreadyExistsException)
                            {
                                var device = registryManager.GetDeviceAsync(deviceId).Result;
                                Console.WriteLine($"Already generated device [{device.Id}] access key: {device.Authentication.SymmetricKey.PrimaryKey}");
                            }
                            else
                            {
                                throw;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    registryManager.CloseAsync().Wait();
                }
                return;
            }
            if (args.Length > 1 && args[0].ToLower() == "remove")
            {
                return;
            }
            Console.WriteLine($"Usage: {Path.GetFileName(typeof(Program).Assembly.Location)} command deviceId...");
            Console.WriteLine("Command");
            Console.WriteLine("  list");
            Console.WriteLine("  add deviceId...");
            Console.WriteLine("  remove deviceId...");
            Console.ReadLine();
        }
    }
}
