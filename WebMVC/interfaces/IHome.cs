using System.Threading.Tasks;

namespace WebMVC.interfaces
{
    public interface IHome
    {
        public Task<string> NameResponse(string Prefix);
    }
}
