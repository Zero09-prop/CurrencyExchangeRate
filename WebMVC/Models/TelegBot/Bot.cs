using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace WebMVC.Models.TelegBot
{
    /// <summary>
    /// Устройство бота
    /// </summary>
    class Bot
    {
        private static TelegramBotClient botClient;
        private static List<Command> commandsList;

        private static IOptions<IDP> _IDPs;
        private static string token ;
        private static string boturl ;
        public Bot(IOptions<IDP> IDPs)
        {
             _IDPs = IDPs;
             token = _IDPs.Value.BotToken;
             boturl = _IDPs.Value.BotUrl;
        }

        public IReadOnlyList<Command> Commands => commandsList.AsReadOnly();

        public static async Task<TelegramBotClient> GetBotClientAsync()
        {
            if (botClient != null)
            {
                return botClient;
            }
            commandsList = new List<Command>();
            //commandsList.Add(new StartCommand());
            //TODO: Add more commands
            botClient = new TelegramBotClient(AppSettings.Key);
            var hook = string.Format(AppSettings.Url, @"api/bot");
            await botClient.SetWebhookAsync(hook);
            return botClient;
        }
    }
}
