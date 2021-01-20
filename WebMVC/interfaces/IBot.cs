using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using WebMVC.Models;

namespace WebMVC.interfaces
{
    public interface IBot
    {
        public void Temp(IOptions<IDP> _options);

    }
}
