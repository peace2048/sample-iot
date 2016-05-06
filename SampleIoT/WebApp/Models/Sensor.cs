using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApp.Models
{
    public class Sensor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Queue<SensorValue> Values { get; set; }
    }
}