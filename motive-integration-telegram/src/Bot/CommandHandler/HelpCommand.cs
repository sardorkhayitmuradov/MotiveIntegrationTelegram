using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace motive_integration_telegram.src.Bot.CommandHandler
{
    public class HelpCommand
    {
        private readonly ITelegramBotClient _bot;

        public HelpCommand(ITelegramBotClient bot)
        {
            _bot = bot;
        }

        public async Task ExecuteAsync(Message message, CancellationToken ct)
        {
            var helpText = "📋 *Available Commands:*\n\n" +
                           "/fuel - Get current fuel information for the vehicle\n" +
                           "/location - Get the current location of the vehicle\n" +
                           "/help - Show this help message\n";

            await _bot.SendMessage(
                chatId: message.Chat.Id,
                text: helpText,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                cancellationToken: ct
            );
        }
    }
}
