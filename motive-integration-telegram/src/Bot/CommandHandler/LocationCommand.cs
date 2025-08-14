using motive_integration_telegram.src.Services;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace motive_integration_telegram.src.Bot.CommandHandler
{
    public class LocationCommand
    {
        private readonly ITelegramBotClient _bot;
        private readonly Message _message;

        public LocationCommand(ITelegramBotClient bot, Message message)
        {
            _bot = bot;
            _message = message;
        }

        public async Task ExecuteAsync()
        {
            var motive = new MotiveApiService();
            var location = await motive.GetVehicleLocationAsync();

            if (location == null)
            {
                await _bot.SendMessage(_message.Chat.Id, "Could not fetch vehicle location.");
                return;
            }

            await _bot.SendLocation(
                _message.Chat.Id,
                location.Latitude,
                location.Longtitude
            );

            await _bot.SendMessage(
                _message.Chat.Id,
                $"Last update: {location.Timestamp:yyyy-MM-dd HH:mm:ss}"
            );
        }

    }
}
