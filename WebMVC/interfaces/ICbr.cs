using Microsoft.Extensions.Options;
using WebMVC.Models;

namespace WebMVC.interfaces
{
    public interface ICbr
    {
        public CbrResponse CbrWork(IOptions<IDP> _options);
    }
}
