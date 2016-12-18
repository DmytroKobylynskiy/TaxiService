using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Identity.Models
{
    public class TaxiOrder
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Недопустимая начальная точка")]
        [RegularExpression(@"^[А-Я]+[а-яА-Я''-'\s]*$", ErrorMessage = "Недопустимая начальная точка")]
        public string StartPoint { get; set; }
        [Required(ErrorMessage = "Недопустимая конечная точка")]
        [RegularExpression(@"^[А-Я]+[а-яА-Я''-'\s]*$", ErrorMessage = "Недопустимая конечная точка")]
        public string EndPoint { get; set; }
        [Required(ErrorMessage = "Недопустимая дата")]
        [Range(typeof(DateTime), "11/12/2016", "1/1/2020")]
        public string Date { get; set; }
        [Required(ErrorMessage = "Недопустимое время")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh:mm tt}")]
        [DataType(DataType.Time)]
        public string Time { get; set; }
        public bool WithAnimals { get; set; }
        public bool FreightCar { get; set; }
        [Required(ErrorMessage = "Расстояние от 1 до 10000")]
        [Range(1,10000,ErrorMessage = "Расстояние от 1 до 10000")]
        public float Distanse { get; set; }
        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh:mm tt}")]
        [DataType(DataType.Time)]
        public float Duration { get; set; }
        [Required]
        [Phone(ErrorMessage = "Недопустимый номер телефона")]
        public string PassengerPhone { get; set; }
        [Required(ErrorMessage = "Недопустимое имя")]
        [RegularExpression(@"^[А-Я]+[а-яА-Я''-'\s]*$", ErrorMessage = "Не имя")]
        [StringLength(60, MinimumLength = 3)]
        public string PassengerName { get; set; }
        public string OrderOwnerId { get; set; }
        public string OrderStatus { get; set; }
        public string ReceiverId { get; set; }
        public DateTime ExpectedTime { get; set; }
    }
}
