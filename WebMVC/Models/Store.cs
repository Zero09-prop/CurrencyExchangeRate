using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.Models
{
    public static class Store
    {
        public static ConcurrentDictionary<string, string> dictValute;
    }
}
