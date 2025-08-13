using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace motive_integration_telegram.src.Services
{
    public class VehicleLocation
    {
        public string VehicleId { get; set; } = "";
        public double Latitude { get; set; }
        public double Longtitude { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
