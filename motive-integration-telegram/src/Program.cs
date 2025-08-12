using System.Text.Json.Nodes;
using System.Text.Json;

namespace motive_integration_telegram.src
{
    public static class Program
    {
        public static async Task Main()
        {
            var apiKey = "";
            var vehicleId = ;

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("accept", "application/json");
            client.DefaultRequestHeaders.Add("x-api-key", apiKey);

            var url = $"https://api.gomotive.com/v1/vehicle_locations?ids={vehicleId}";

            try
            {
                var json = await client.GetStringAsync(url);

                var doc = JsonNode.Parse(json);
                var filteredVehicle = doc?["vehicles"]?
                    .AsArray()
                    .FirstOrDefault(v => v?["vehicle"]?["id"]?.ToString() == vehicleId.ToString());

                Console.WriteLine(filteredVehicle?.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP error: {ex.Message}");
            }
        }
    }
}