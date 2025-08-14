using motive_integration_telegram.src.Services.Models;
using motive_integration_telegram.src.Utils;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace motive_integration_telegram.src.Services
{
    public class MotiveApiService
    {
        private readonly string _apiKey;
        private readonly string _vehicleId;
        private readonly HttpClient _httpClient;


        public MotiveApiService(string apiKey)
        {
            _apiKey = ConfigHelper.GetEnvVar("MOTIVE_API_KEY");

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("accept", "application/json");
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
        }

        public MotiveApiService()
        {
            _apiKey = ConfigHelper.GetEnvVar("MOTIVE_API_KEY");

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("accept", "application/json");
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _apiKey);
        }

        public async Task<List<FuelInfo>?> GetAllFuelInfoAsync()
        {
            var fuelList = new List<FuelInfo>();
            var url = $"https://api.gomotive.com/v1/vehicles";

            try
            {
                var json = await _httpClient.GetStringAsync(url);
                var doc = JsonNode.Parse(json);

                var vehicles = doc?["vehicles"]?.AsArray();
                if (vehicles == null)
                {
                    return fuelList;
                }

                foreach (var v in vehicles)
                {
                    var unitNumber = v?["number"]?.ToString() ?? "Unknown";
                    var fuelStr = v?["current_location"]?["fuel_primary_remaining_percentage"]?.ToString();
                    var timestampStr = v?["current_location"]?["updated_at"]?.ToString();

                    double fuelPct = 0;
                    if (!string.IsNullOrWhiteSpace(fuelStr))
                        double.TryParse(fuelStr, out fuelPct);

                    DateTime timestamp = DateTime.MinValue;
                    if (!string.IsNullOrWhiteSpace(timestampStr))
                    {
                        _ = DateTime.TryParse(timestampStr, out timestamp);
                    }

                    fuelList.Add(new FuelInfo
                    {
                        VehicleId = unitNumber,
                        FuelPercentage = fuelPct,
                        Timestamp = timestamp
                    });
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP error: {ex.Message}");
                return null;
            }

            return fuelList;
        }

        public async Task<VehicleLocation> GetVehicleLocationAsync()
        {
            var url = $"https://api.gomotive.com/v1/vehicle_locations?ids={_vehicleId}";

            try
            {
                var json = await _httpClient.GetStringAsync(url);

                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                // Assuming API returns: { "vehicles": [ { "vehicle": { "id": "...", "latitude": ..., "longitude": ..., "timestamp": ... } } ] }
                var vehicleElement = root
                    .GetProperty("vehicles")[0]
                    .GetProperty("vehicle");

                var location = new VehicleLocation
                {
                    VehicleId = vehicleElement.GetProperty("id").GetString() ?? "",
                    Latitude = vehicleElement.GetProperty("latitude").GetDouble(),
                    Longtitude = vehicleElement.GetProperty("longitude").GetDouble(),
                    Timestamp = vehicleElement.GetProperty("timestamp").GetDateTime()
                };

                return location;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP error calling Motive API: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing vehicle location: {ex.Message}");
                return null;
            }
        }
    }
}
