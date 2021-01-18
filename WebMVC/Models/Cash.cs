using System;
using System.Collections.Concurrent;

namespace WebMVC.Models
{
    /// <summary>
    /// Кеш сервера 
    /// </summary>
    public class Cash
    {
        private static int initialCapacity = 79;
        private static int numProcs = Environment.ProcessorCount;
        private static int concurrencyLevel = numProcs * 2;
        public ConcurrentDictionary<string, Currency> CashValues = new ConcurrentDictionary<string, Currency>(concurrencyLevel, initialCapacity);
    }
}
