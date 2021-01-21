using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace WebMVC.Models.TelegBot
{
    /// <summary>
    /// Устройство бота
    /// </summary>
    public  class Bot
    {
        private TelegramBotClient botClient;
        private readonly IOptions<IDP> _IDPs;
        public Bot(IOptions<IDP> IDPs)
        {
            _IDPs = IDPs;
        }

        public async Task<TelegramBotClient> GetBotClientAsync()
        {
            if (botClient != null)
            {
                return botClient;
            }
            botClient = new TelegramBotClient(_IDPs.Value.BotToken);
            var hook = string.Format(_IDPs.Value.BotUrl, @"api/bot");
            await botClient.SetWebhookAsync(hook);
            return botClient;
        }
    }
}
