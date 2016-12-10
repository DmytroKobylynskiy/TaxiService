using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Identity.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool CarExist { get; set; }
        public string DriverLicense { get; set; }
    }
}
