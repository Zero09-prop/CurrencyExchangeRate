using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using Telegram.Bot.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.interfaces
{
    public interface IHome
    {
        public Task<string> NameResponse(string Prefix);
    }
}
