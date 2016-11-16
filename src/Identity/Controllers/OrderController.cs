using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity.Data;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Identity.Controllers
{
    public class OrderController : Controller
    {
        UserManager<ApplicationUser> _userManager;
        //private ApplicationDbContext db;
        public async Task TaxiOrders()
        {
            
        }
        public IActionResult CreateTaxiOrder()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateTaxiOrder(TaxiOrder taxiOrder)
        {
            Console.WriteLine(_userManager.Users.ToList());
            //var user = await GetCurrentUserAsync();
            //var userId = user?.Id;
            //taxiOrder.OrderOwnerId=userId;
            //db.TaxiOrders.Add(taxiOrder);
            // await db.SaveChangesAsync();
            return RedirectToAction("TaxiOrders");
        }

    }
}
