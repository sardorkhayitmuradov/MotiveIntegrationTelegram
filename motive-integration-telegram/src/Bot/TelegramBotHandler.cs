using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot;
using motive_integration_telegram.src.Bot.CommandHandler;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.Enums;

namespace src.Bot
{
    public class TelegramBotHandler
    {
        private readonly ITelegramBotClient _botClient;

        public TelegramBotHandler(string token)
        {
            _botClient = new TelegramBotClient(token);
        }

        public async Task StartAsync()
        {
            var me = await _botClient.GetMe();
            Console.WriteLine($"🤖 Connected as @{me.Username}");

            using var cts = new CancellationTokenSource();

            _botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                new Telegram.Bot.Polling.ReceiverOptions
                {
                    AllowedUpdates = Array.Empty<UpdateType>()
                },
                cancellationToken: cts.Token
            );

            // Keep running until cancelled
            await Task.Delay(-1, cts.Token);
        }

        private async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
        {
            if (update.Type != UpdateType.Message || update.Message?.Text == null)
                return;

            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text.Trim();

            Console.WriteLine($"📩 Received from {chatId}: {messageText}");

            try
            {
                switch (messageText.Split(' ')[0].ToLower())
                {
                    case "/fuel":
                        await new FuelCommand(bot).ExecuteAsync(update.Message, ct);
                        break;

                    case "/location":
                        await new LocationCommand(bot).ExecuteAsync(update.Message, ct);
                        break;

                    case "/help":
                        await new HelpCommand(bot).ExecuteAsync(update.Message, ct);
                        break;

                    default:
                        await bot.SendMessage(chatId, "❓ Unknown command. Type /help for a list of commands.", cancellationToken: ct);
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Error handling update: {ex.Message}");
                await bot.SendMessage(chatId, "⚠️ Something went wrong. Please try again later.", cancellationToken: ct);
            }
        }

        private Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken ct)
        {
            var errorMsg = exception switch
            {
                ApiRequestException apiEx => $"Telegram API Error:\n[{apiEx.ErrorCode}] {apiEx.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine($"❌ {errorMsg}");
            return Task.CompletedTask;
        }
    }
}
