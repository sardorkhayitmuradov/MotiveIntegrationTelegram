using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot;
using motive_integration_telegram.src.Bot.CommandHandler;

namespace src.Bot
{
    public class TelegramBotHandler
    {
        private readonly ITelegramBotClient _bot;

        public TelegramBotHandler()
        {
            var token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN")
                        ?? throw new DirectoryNotFoundException("TELEGRAM_BOT_TOKEN not set in .env");
            _bot = new TelegramBotClient(token);
        }

        public async Task StartAsync()
        {
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }
            };

            _bot.StartReceiving(OnMessage, OnError, receiverOptions);
            var me = await _bot.GetMe();
            Console.WriteLine($"Bot @{me.Username} started!");
        }

        private static async Task OnMessage(ITelegramBotClient bot, Update update, CancellationToken ct)
        {
            if (update.Message is not { } message || message.Text is null) return;

            var text = message.Text.Trim().ToLower();

            switch (text)
            {
                case "/location":
                    await new LocationCommand(bot, message).ExecuteAsync();
                    break;
            }
        }

        private static Task OnError(ITelegramBotClient bot, Exception ex, CancellationToken ct)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return Task.CompletedTask;
        }
    }
}
