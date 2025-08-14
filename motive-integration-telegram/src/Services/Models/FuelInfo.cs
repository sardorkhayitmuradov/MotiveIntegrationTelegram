using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace motive_integration_telegram.src.Services.Models
{
    public class FuelInfo
    {
        public string VehicleId { get; set; } = "";
        public double FuelPercentage { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
