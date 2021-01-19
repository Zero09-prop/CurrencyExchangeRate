using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMVC.Models;

namespace WebMVC.abs_classes
{
    public class ACash
    {
        public Cash cash;

        public ACash()
        {
            cash = Cash.GetInstance();
        }
    }
}
