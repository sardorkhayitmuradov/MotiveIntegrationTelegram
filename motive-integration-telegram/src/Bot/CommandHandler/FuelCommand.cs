using motive_integration_telegram.src.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace motive_integration_telegram.src.Bot.CommandHandler
{
    public class FuelCommand
    {
        private readonly ITelegramBotClient _botClient;

        public FuelCommand(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        public async Task ExecuteAsync(Message message)
        {
            var chatId = message.Chat.Id;

            try
            {
                var apiKey = Environment.GetEnvironmentVariable("MOTIVE_API_KEY")
                    ?? throw new NotImplementedException("MOTIVE_API_KEY not set in environment variables.");

                // Create the service only here
                var motiveApiService = new MotiveApiService(apiKey);

                var vehicles = await motiveApiService.GetVehicleLocationAsync();

                if (vehicles == null || vehicles.Count == 0)
                {
                    await _botClient.SendMessage(chatId, "No vehicle data available.", cancellationToken: ct);
                    return;
                }

                var sb = new StringBuilder();
                sb.AppendLine("🚛 **Fuel Levels**\n");

                foreach (var v in vehicles)
                {
                    sb.AppendLine($"Unit: {v.UnitNumber}");
                    sb.AppendLine($"Fuel: {v.FuelPercentage}%");
                    sb.AppendLine($"Date: {v.LastUpdated:yyyy-MM-dd HH:mm}");
                    sb.AppendLine();
                }

                await _botClient.SendMessage(
                    chatId,
                    sb.ToString(),
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    cancellationToken: ct
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error in FuelCommand: {ex.Message}");
                await _botClient.SendMessage(chatId, "⚠️ Failed to retrieve fuel data.", cancellationToken: ct);
            }
        }
    }
}
