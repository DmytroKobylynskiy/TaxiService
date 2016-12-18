using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Identity.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public bool CarExist { get; set; }
        [Required]
        [StringLength(10)]
        public string DriverLicense { get; set; }

        public string IsAvaliable { get; set; }
    }
}
