using System.ComponentModel.DataAnnotations;
using Microsoft.DotNet.ProjectModel;

namespace Identity.Models
{
    public class TaxiOffer
    {
        public int Id { get; set; }
        [Required]
        [RegularExpression(@"/^[a-zA-Zа-яА-Я'][a-zA-Zа-яА-Я-' ]+[a-zA-Zа-яА-Я']?$/u", ErrorMessage = "Недопустимое имя")]
        public string Name { get; set; } // имя таксиста
        [Required(ErrorMessage = "Не указана машина")]
        public string Auto { get; set; } // машина
        public string Place { get; set; } //расположение
        [Required]
        [Range(10,50, ErrorMessage = "Диапазон цен 10-50грн")]
        public int Price { get; set; } // цена
        public string OfferOwnerId { get; set; }
        public string OfferStatus { get; set; }
    }
}
