using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.interfaces
{
    public interface ICash
    {
        public void CashNew();
        public ConcurrentDictionary<string, double> CashWork(string ValuteCode, ref bool isOk);
    }
}
