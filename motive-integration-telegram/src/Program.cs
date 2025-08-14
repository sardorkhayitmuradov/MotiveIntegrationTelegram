using motive_integration_telegram.src.Utils;
using src.Bot;

namespace motive_integration_telegram.src
{
    public static class Program
    {
        public static async Task Main()
        {
            Console.WriteLine("Motive Integration Telegram bot starting...");

            try
            {
                var telegramToken = ConfigHelper.GetEnvVar("TELEGRAM_BOT_TOKEN");

                if (string.IsNullOrWhiteSpace(telegramToken))
                {
                    throw new NotImplementedException("TELEGRAM_BOT_TOKEN is missing in environment variables.");
                }

                var botHandler = new TelegramBotHandler(telegramToken);
                await botHandler.StartAsync();

                Console.WriteLine("Bot is running. Press Ctrl+C to exit.");
                await Task.Delay(-1);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fatal error: {ex.Message}");
            }
        }
    }
}