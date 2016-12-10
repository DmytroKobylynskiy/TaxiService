using System.Collections.Generic;
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
        UserManager<ApplicationUser> userManager;
        private ApplicationDbContext db;
        private Task<ApplicationUser> GetCurrentUserAsync() => userManager.GetUserAsync(HttpContext.User);

        public OrderController(ApplicationDbContext context, UserManager<ApplicationUser> _userManager)
        {
            db = context;
            userManager = _userManager;
        }

        public async Task<IActionResult> TaxiOrders()
        {
            List<TaxiOrder> taxiOrders = await db.TaxiOrders.ToListAsync();
            List<TaxiOrder> taxi = new List<TaxiOrder>();
            for (int i = 0; i < taxiOrders.Count; i++)
            {
                if (taxiOrders[i].OrderStatus!="Done")
                {
                    taxi.Add(taxiOrders[i]);
                }
            }
            return View(taxi);
        }

        public async Task<IActionResult> MyOrders()
        {
            var user = await GetCurrentUserAsync();
            var userId = user?.Id;
            List<TaxiOrder> allOrders = await db.TaxiOrders.ToListAsync();
            List<TaxiOrder> myOrders = new List<TaxiOrder>();
            foreach (var order in allOrders)
            {
                if (order.OrderOwnerId == userId)
                {
                    myOrders.Add(order);
                }
            }
            return View(myOrders);
        }


        public IActionResult CreateTaxiOrder()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateTaxiOrder(TaxiOrder taxiOrder)
        {

            var user = await GetCurrentUserAsync();
            var userId = user?.Id;
            taxiOrder.OrderOwnerId = userId;
            taxiOrder.OrderStatus = "Free";
            db.TaxiOrders.Add(taxiOrder);
            await db.SaveChangesAsync();
            return RedirectToAction("TaxiOrders");
        }

        public async Task<IActionResult> DetailsTaxiOrder(int? id)
        {
            if (id != null)
            {
                TaxiOrder taxiOrder = await db.TaxiOrders.FirstOrDefaultAsync(p => p.Id == id);
                if (taxiOrder != null)
                    return View(taxiOrder);
            }
            return NotFound();
        }

        public async Task<IActionResult> EditTaxiOrder(int? id)
        {
            if (id != null)
            {
                TaxiOrder taxiOrder = await db.TaxiOrders.FirstOrDefaultAsync(p => p.Id == id);
                if (taxiOrder != null)
                    return View(taxiOrder);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> EditTaxiOrder(TaxiOrder taxiOrder)
        {
            db.TaxiOrders.Update(taxiOrder);
            await db.SaveChangesAsync();
            return RedirectToAction("TaxiOrders");
        }

        
        public async Task<IActionResult> DeleteTaxiOrder()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTaxiOrder(int? id)
        {
            if (id != null)
            {
                TaxiOrder taxiOrder = await db.TaxiOrders.FirstOrDefaultAsync(p => p.Id == id);
                if (taxiOrder != null)
                {
                    db.TaxiOrders.Remove(taxiOrder);
                    await db.SaveChangesAsync();
                    return RedirectToAction("TaxiOrders");
                }
            }
            return NotFound();
        }

        public async Task<IActionResult> AgreeTaxiOrder(int? id)
        {
            if (id != null)
            {
                TaxiOrder taxiOrder = await db.TaxiOrders.FirstOrDefaultAsync(p => p.Id == id);
                if (taxiOrder != null)
                {
                    taxiOrder.OrderStatus = "In progress";
                    db.TaxiOrders.Update(taxiOrder);
                    await db.SaveChangesAsync();
                    return View(taxiOrder);
                }
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmTaxiOrder(int? id, TaxiOrder taxi)
        {
            if (id != null)
            {
                TaxiOrder taxiOrder = await db.TaxiOrders.FirstOrDefaultAsync(p => p.Id == id);
                if (taxiOrder != null)
                {
                    
                    taxiOrder.Distanse = taxi.Distanse;
                    taxiOrder.Duration = taxi.Duration;
                    taxiOrder.OrderStatus = "Done";
                    db.TaxiOrders.Update(taxiOrder);
                    await db.SaveChangesAsync();
                    return RedirectToAction("TaxiOrders");
                }
            }
            return NotFound();
        }

        public async Task<IActionResult> ConfirmTaxiOrder(int? id)
        {
            TaxiOrder taxiOrder = await db.TaxiOrders.FirstOrDefaultAsync(p => p.Id == id);
            return View(taxiOrder);
        }

        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDeleteTaxiOrder(int? id)
        {
            if (id != null)
            {
                TaxiOrder taxiOrder = await db.TaxiOrders.FirstOrDefaultAsync(p => p.Id == id);
                if (taxiOrder != null)
                    return View(taxiOrder);
            }
            return NotFound();
        }
    }
}
