using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.Models
{
    public class TaxiOrder
    {
        public int Id { get; set; }
        public string StartPoint { get; set; }
        public string EndPoint { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public bool WithAnimals { get; set; }
        public bool FreightCar { get; set; }
        public float Distanse { get; set; }
        public float Duration { get; set; }
        public string PassengerPhone { get; set; }
        public string PassengerName { get; set; }
        public string OrderOwnerId { get; set; }
        public string OrderStatus { get; set; }
    }
}
