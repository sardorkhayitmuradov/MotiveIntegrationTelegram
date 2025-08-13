using System.Text.Json.Nodes;

namespace motive_integration_telegram.src.Services
{
    public class MotiveApiService
    {
        private readonly string _apiKey;
        private readonly string _vehicleId;
        private readonly HttpClient _client;


        public MotiveApiService()
        {
            _apiKey = Environment.GetEnvironmentVariable("MOTIVE_API_KEY")
                ?? throw new Exception("MOTIVE_API_KEY not set in .env");

            _vehicleId = Environment.GetEnvironmentVariable("MOTIVE_VEHICLE_ID")
                ?? throw new Exception("MOTIVE_VEHICLE_ID not set in .env");

            _client = new HttpClient();
            _client.DefaultRequestHeaders.Add("accept", "application/json");
            _client.DefaultRequestHeaders.Add("x-api-key", _apiKey);
        }

        public async Task<VehicleLocation?> GetVehicleLocationAsync()
        {
            var url = $"https://api.gomotive.com/v1/vehicle_locations?ids={_vehicleId}";

            try
            {
                var json = await _client.GetStringAsync(url);
                var doc = JsonNode.Parse(json);

                var filteredVehicle = doc?["vehicles"]?
                    .AsArray()
                    .FirstOrDefault(v => v?["vehicle"]?["id"]?.ToString() == _vehicleId);

                if (filteredVehicle is null)
                    return null;

                return new VehicleLocation
                {
                    VehicleId = _vehicleId,
                    Latitude = filteredVehicle["location"]?["lat"]?.GetValue<double>() ?? 0,
                    Longtitude = filteredVehicle["location"]?["lon"]?.GetValue<double>() ?? 0,
                    Timestamp = filteredVehicle["location"]?["recorded_at"]?.GetValue<DateTime>() ?? DateTime.MinValue
                };
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP error: {ex.Message}");
                return null;
            }
        }
    }
}
