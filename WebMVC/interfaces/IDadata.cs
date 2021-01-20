using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebMVC.interfaces
{
    public interface IDadata
    {
        public Task<string> DadataResponse(string Prefix);
    }
}
