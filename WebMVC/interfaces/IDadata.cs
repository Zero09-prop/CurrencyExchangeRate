using System.Threading.Tasks;

namespace WebMVC.interfaces
{
    public interface IDadata
    {
        public Task<string> DadataResponse(string Prefix);
    }
}
