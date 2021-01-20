using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using WebMVC.Models;

namespace WebMVC.interfaces
{
    public interface ICbr
    {
        public CbrResponse CbrWork(IOptions<IDP> _options);
    }
}
