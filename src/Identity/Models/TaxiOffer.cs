using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.Models
{
    public class TaxiOffer
    {
        public int Id { get; set; }
        public string Name { get; set; } // имя таксиста
        public string Auto { get; set; } // машина
        public string Place { get; set; } //расположение
        public int Price { get; set; } // цена
    }
}
