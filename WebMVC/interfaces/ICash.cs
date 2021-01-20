using System.Collections.Concurrent;

namespace WebMVC.interfaces
{
    public interface ICash
    {
        public void CashNew();
        public void CashClean();
        public ConcurrentDictionary<string, double> CashWork(string ValuteCode, ref bool isOk);
    }
}
