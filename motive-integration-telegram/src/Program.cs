using motive_integration_telegram.src.Services;
using motive_integration_telegram.src.Utils;

namespace motive_integration_telegram.src
{
    public static class Program
    {
        public static async Task Main()
        {
            var apiKey = ConfigHelper.GetEnvVar("MOTIVE_API_KEY");
            var botToken = ConfigHelper.GetEnvVar("TELEGRAM_BOT_TOKEN");

            var motiveService = new MotiveApiService(apiKey);
            var botService = new TelegramBotService(botToken, motiveService);

            await botService.StartAsync();
                Console.WriteLine($"HTTP error: {ex.Message}");
            }
                Console.WriteLine($"HTTP error: {ex.Message}");
            }
        }
    }
}