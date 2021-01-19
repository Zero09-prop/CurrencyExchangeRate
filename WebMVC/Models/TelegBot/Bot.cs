using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;

namespace WebMVC.Models.TelegBot
{
    /// <summary>
    /// Устройство бота
    /// </summary>
    class Bot
    {
        private static Bot bot;
        private TelegramBotClient botClient;
        private List<Command> commandsList;

        private Bot() { }

        public IReadOnlyList<Command> Commands => commandsList.AsReadOnly();

        public static Bot getInstance()
        {
            if (bot == null)
                bot = new Bot();
            return bot;
        }

        public async Task<TelegramBotClient> GetBotClientAsync()
        {
            if (botClient != null)
            {
                return botClient;
            }

            commandsList = new List<Command>();
            //commandsList.Add(new StartCommand());
            //TODO: Add more commands

            botClient = new TelegramBotClient(AppSettings.Key);
            var hook = string.Format(AppSettings.Url,@"api/bot");
            await botClient.SetWebhookAsync(hook);
            return botClient;
        }
    }
}
