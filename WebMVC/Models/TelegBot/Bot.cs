using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using WebMVC.interfaces;

namespace WebMVC.Models.TelegBot
{
    /// <summary>
    /// Устройство бота
    /// </summary>
    class Bot : IBot
    {
        private static TelegramBotClient botClient;
        private static IOptions<IDP> _IDPs;

        public void Temp(IOptions<IDP> IDPs)
        {
            _IDPs = IDPs;
        }

        public static async Task<TelegramBotClient> GetBotClientAsync()
        {
            if (botClient != null)
            {
                return botClient;
            }
            botClient = new TelegramBotClient(AppSettings.Key);
            var hook = string.Format(AppSettings.Url, @"api/bot");
            await botClient.SetWebhookAsync(hook);
            return botClient;
        }
    }
}
