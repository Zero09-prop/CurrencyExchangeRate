using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebMVC.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using WebMVC.interfaces;
using WebMVC.Models.TelegBot;

namespace WebMVC.Controllers
{
    public sealed class TelegramBotController : Controller
    {

        private ICash icash;
        private IHome ihome;
        public TelegramBotController(ICash icash, IHome ihome)
        {
            this.icash = icash;
            this.ihome = ihome;
        }

        /// <summary>
        /// Метод для принятия сообщений от бота
        /// </summary>
        /// <param name="update"></param>
        /// <returns>Возращает запрошенный курс валют или ошибку</returns>
        
        [Route("api/bot")]
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] Update update)
        {
            //Инициализировали клиент
            //
            TelegramBotClient client = await Bot.GetBotClientAsync();
            if (update == null)
            {
                return NotFound("Error message");
            }

            //Взяли id сообщения и чата
            //

            var chatId = update.Message.Chat.Id;
            var messageId = update.Message.MessageId;

            //Вызвали функцию для работы с сайтом Dadata, для того чтобы она установила значения в словарь
            //dictValute. В последствии строка empty нам не понадобится
            //
            var empty = ihome.NameResponse(update.Message.Text).Result;
            string ValuteCode = "";

            //пробегаемя по всем ключам,которые могли выпасть при обращению к Dadata,
            //как правило это один ключ. Если отправленное боту сообщение содержит нечеткое выражение,
            // например "доллар", то в таком случае dictValute содержит несколько валют со словом доллар
            // но по условию задачи на вход пойдет либо код валюты,либо её полное имя

            foreach (var key in Store.dictValute.Keys)
            {
                ValuteCode = Store.dictValute[key];
            }

            //Проверка на наличие ошибок
            //

            bool isOk = false;

            //Обращаемя к кешу,а затем и к сайту ЦБ
            //

            ConcurrentDictionary<string, double> dictionary = icash.CashWork(ValuteCode, ref isOk);

            //Если ошибка,то отправляем пользователю сообщение
            //

            if (!isOk)
            {
                await client.SendTextMessageAsync(chatId, "Ваша валюта не представлена на сайте ЦБ",
                    replyToMessageId: messageId);
                return Ok();
            }

            //В противном случае словарь dictionary содержит один экземпляр валюты
            //
            string Name = "";
            double Value = 0.0;
            foreach (var element in dictionary.Keys)
            {
                Name = element;
                Value = dictionary[element];
            }

            await client.SendTextMessageAsync(chatId, $"Курс вашей валюты {Name} равен {Value}",
                replyToMessageId: messageId);
            return Ok("its good and okey");
        }
    }
}
