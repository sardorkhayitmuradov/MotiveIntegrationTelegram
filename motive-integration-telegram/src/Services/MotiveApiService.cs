using motive_integration_telegram.src.Services.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.Json.Nodes;

namespace motive_integration_telegram.src.Services
{
    public class MotiveApiService
    {
        private readonly HttpClient _client;

        public MotiveApiService(string apiKey)
        {
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("accept", "application/json");
            _client.DefaultRequestHeaders.Add("x-api-key", apiKey);
        }

        public async Task<List<VehicleInfo>> GetAllVehicleLocationsAsync()
        {
            var list = new List<VehicleInfo>();
            var json = await _client.GetStringAsync($"https://api.gomotive.com/v1/vehicle_locations");
            var doc = JsonNode.Parse(json);
            var vehicles = doc?["vehicles"]?.AsArray();

            if (vehicles == null) return list;

            foreach (var v in vehicles)
            {
                var vehicle = v?["vehicle"];
                if (vehicle == null) continue;

                double? fuel = vehicle["current_location"]?["fuel_primary_remaining_percentage"]?.GetValue<double?>()
                               ?? vehicle["fuel_primary_remaining_percentage"]?.GetValue<double?>();

                double? latitude = vehicle["current_location"]?["lat"]?.GetValue<double?>();
                double? longitude = vehicle["current_location"]?["lon"]?.GetValue<double?>();

                string fuelList;
                if (fuel.HasValue)
                {
                    var rounded = Math.Round(fuel.Value);
                    if (Math.Abs(fuel.Value - rounded) < 1e-6)
                    {
                        fuelList = ((int)rounded).ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        fuelList = fuel.Value.ToString("0.###", CultureInfo.InvariantCulture);
                    }
                }
                else
                {
                    fuelList = "OFF";
                }

                list.Add(new VehicleInfo
                {
                    Id = vehicle["id"]?.GetValue<int>() ?? 0,
                    Number = vehicle["number"]?.ToString() ?? "",
                    Make = vehicle["make"]?.ToString() ?? "",
                    Model = vehicle["model"]?.ToString() ?? "",
                    Vin = vehicle["vin"]?.ToString() ?? "",
                    FuelPercent = fuelList,
                    Latitude = latitude,
                    Longitude = longitude
                });
            }

            return list;
        }
    }
}
