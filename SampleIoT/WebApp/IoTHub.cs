using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Collections.Concurrent;
using WebApp.Models;

namespace WebApp
{
    public class IoTHub : Hub
    {
        private static Dictionary<int, Sensor> _sensors = new Dictionary<int, Sensor> {
            { 1, new Sensor { Id = 1, Name = "1F", Values = new Queue<SensorValue>() } }
        };

        public static List<Sensor> GetCurrentValues()
        {
            return _sensors.Values.ToList();
        }

        public void SendValue(int id, DateTime time, double value)
        {
            Sensor sensor;
            if (_sensors.TryGetValue(id, out sensor))
            {
                sensor.Values.Enqueue(new SensorValue { Time = time, Value = value });
                if (sensor.Values.Count > 50)
                {
                    sensor.Values.Dequeue();
                }
            }
            Clients.All.addValue(id, time, value);
        }
    }
}