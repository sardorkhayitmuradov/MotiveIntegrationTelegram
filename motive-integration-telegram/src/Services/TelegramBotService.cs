using System.Net;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace motive_integration_telegram.src.Services
{
    public class TelegramBotService
    {
        private readonly ITelegramBotClient _botClient;
        private readonly MotiveApiService _motiveService;

        public TelegramBotService(string botToken, MotiveApiService motiveService)
        {
            _botClient = new TelegramBotClient(botToken);
            _motiveService = motiveService;
        }

        public async Task StartAsync()
        {
            var me = await _botClient.GetMe();
            Console.WriteLine($"Bot {me.Username} is running...");

            _botClient.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync
            );

            await Task.Delay(-1);
        }

        private async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
        {
            if (update.Type != UpdateType.Message || update.Message!.Text == null)
                return;

            var chatId = update.Message.Chat.Id;
            var messageText = update.Message.Text.Trim().ToLower();

            if (messageText == "/start")
            {
                await bot.SendMessage(chatId, "Welcome! Send /fuel to get truck fuel levels.", cancellationToken: ct);
            }
            else if (messageText == "/fuel")
            {
                var vehicles = (await _motiveService.GetAllVehicleLocationsAsync())
                    .OrderBy(v =>
                    {
                        if (double.TryParse(v.FuelPercent, out var fuel))
                        {
                            return fuel;
                        }
                        return double.MaxValue;
                    }).ToList();
                if (!vehicles.Any())
                {
                    await bot.SendMessage(chatId, "No vehicles found.", cancellationToken: ct);
                    return;
                }

                string Encode(string? s) => WebUtility.HtmlEncode(s ?? "");

                var lines = vehicles.Select(v =>
                {
                    var number = Encode(v.Number);
                    var fuel = Encode(v.FuelPercent);
                    var lat = v.Latitude.HasValue ? v.Latitude.Value.ToString("G7") : "N/A";
                    var lon = v.Longitude.HasValue ? v.Longitude.Value.ToString("G7") : "N/A";
                    var coords = WebUtility.HtmlEncode($"{lat}, {lon}");
                    return $"🚚 {number} → ⛽️ Fuel: {fuel}% → 📍<code>{coords}</code>";
                });

                var message = string.Join("\n", lines);

                await _botClient.SendMessage(
                    chatId,
                    message,
                    parseMode: ParseMode.Html,
                    cancellationToken: ct
                );
            }
        }

        private static Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken ct)
        {
            Console.WriteLine($"Bot Error: {exception.Message}");
            return Task.CompletedTask;
        }
    }
}
