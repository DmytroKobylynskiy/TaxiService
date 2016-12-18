using System.ComponentModel.DataAnnotations;
using Microsoft.DotNet.ProjectModel;

namespace Identity.Models
{
    public class TaxiOffer
    {
        public int Id { get; set; }
        [RegularExpression(@"^[А-Я]+[а-яА-Я''-'\s]*$", ErrorMessage = "Не имя")]
        [Required(ErrorMessage = "Не указано имя")]
        public string Name { get; set; } // имя таксиста
        [Required(ErrorMessage = "Не указана машина")]
        public string Auto { get; set; } // машина
        public string Latitude { get; set; } //расположение
        public string Longitude { get; set; }
        [Required(ErrorMessage = "Не указана цена")]
        [Range(10,50, ErrorMessage = "Диапазон цен 10-50грн")]
        public int Price { get; set; } // цена
        public string OfferOwnerId { get; set; }
        public string OfferStatus { get; set; }
    }
}
