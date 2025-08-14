using motive_integration_telegram.src.Services;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace motive_integration_telegram.src.Bot.CommandHandler
{
    public class LocationCommand
    {
        private readonly ITelegramBotClient _bot;

        public LocationCommand(ITelegramBotClient bot)
        {
            _bot = bot;
        }

        public async Task ExecuteAsync(Message message, CancellationToken ct)
        {
            try
            {
                var motiveService = new MotiveApiService();
                var location = await motiveService.GetVehicleLocationAsync();

                if (location == null)
                {
                    await _bot.SendMessage(
                        message.Chat.Id,
                        "⚠️ Could not retrieve vehicle location.",
                        cancellationToken: ct
                    );
                    return;
                }

                var response = $"📍 *Vehicle Location:*\n" +
                               $"Vehicle ID: {location.VehicleId}\n" +
                               $"Latitude: {location.Latitude}\n" +
                               $"Longitude: {location.Longtitude}\n" +
                               $"Updated: {location.Timestamp:yyyy-MM-dd HH:mm}";

                await _bot.SendMessage(
                    message.Chat.Id,
                    response,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    cancellationToken: ct
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ LocationCommand error: {ex.Message}");
                await _bot.SendMessage(
                    message.Chat.Id,
                    "❌ Error retrieving location. Please try again later.",
                    cancellationToken: ct
                );
            }
        }

    }
}
