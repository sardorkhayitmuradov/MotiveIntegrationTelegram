using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace motive_integration_telegram.src.Services.Models
{
    public class VehicleInfo
    {
        public int Id { get; set; }
        public string? Number { get; set; }
        public string? Make { get; set; }
        public string? Model { get; set; }
        public string? Vin { get; set; }
        public string FuelPercent { get; set; } = "N/A";
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
