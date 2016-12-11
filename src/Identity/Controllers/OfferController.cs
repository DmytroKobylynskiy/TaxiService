using System.Collections.Generic;
using System.Threading.Tasks;
using Identity.Data;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Identity.Controllers
{
    public class OfferController : Controller
    {
        private ApplicationDbContext db;
        private UserManager<ApplicationUser> userManager;

        private Task<ApplicationUser> GetCurrentUserAsync() => userManager.GetUserAsync(HttpContext.User);
        public OfferController(ApplicationDbContext context, UserManager<ApplicationUser> _userManager)
        {
            db = context;
            userManager = _userManager;
        }
        public async Task<IActionResult> TaxiOffers()
        {
            return View(await db.TaxiOffers.ToListAsync());
        }
        public IActionResult CreateTaxiOffer()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateTaxiOffer(TaxiOffer taxiOffer)
        {
            var user = await GetCurrentUserAsync();
            var userId = user?.Id;
            taxiOffer.OfferOwnerId = userId;
            db.TaxiOffers.Add(taxiOffer);
            await db.SaveChangesAsync();
            return RedirectToAction("TaxiOffers");
        }

        public async Task<IActionResult> MyOffers()
        {
            var user = await GetCurrentUserAsync();
            var userId = user?.Id;
            List<TaxiOffer> allOrders = await db.TaxiOffers.ToListAsync();
            List<TaxiOffer> myOffers = new List<TaxiOffer>();
            foreach (var order in allOrders)
            {
                if (order.OfferOwnerId == userId)
                {
                    myOffers.Add(order);
                }
            }
            return View(myOffers);
        }

        public async Task<IActionResult> DeleteTaxiOffer(int id)
        {
            if (id != null)
            {
                TaxiOffer taxiOffer = await db.TaxiOffers.FirstOrDefaultAsync(p => p.Id == id);
                if (taxiOffer != null)
                    return View(taxiOffer);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTaxiOffer(int? id)
        {
            if (id != null)
            {
                TaxiOffer offer = await db.TaxiOffers.FirstOrDefaultAsync(p => p.Id == id);
                if (offer != null)
                {
                    db.TaxiOffers.Remove(offer);
                    await db.SaveChangesAsync();
                    return RedirectToAction("TaxiOffers");
                }
            }
            return NotFound();
        }

        public async Task<IActionResult> DetailsTaxiOffer(int? id)
        {
            if (id != null)
            {
                TaxiOffer taxiOffer = await db.TaxiOffers.FirstOrDefaultAsync(p => p.Id == id);
                if (taxiOffer != null)
                    return View(taxiOffer);
            }
            return NotFound();
        }


        public async Task<IActionResult> EditTaxiOffer(int? id)
        {
            if (id != null)
            {
                TaxiOffer taxiOffer = await db.TaxiOffers.FirstOrDefaultAsync(p => p.Id == id);
                if (taxiOffer != null)
                    return View(taxiOffer);
            }
            return NotFound();
        }


        [HttpPost]
        public async Task<IActionResult> EditTaxiOffer(TaxiOffer taxiOffer)
        {
            db.TaxiOffers.Update(taxiOffer);
            await db.SaveChangesAsync();
            return RedirectToAction("TaxiOffers");
        }


        public async Task<IActionResult> AgreeTaxiOffer(int? id)
        {
            if (id != null)
            {
                TaxiOffer taxiOffer = await db.TaxiOffers.FirstOrDefaultAsync(p => p.Id == id);
                if (taxiOffer != null)
                {
                    taxiOffer.OfferStatus = "In progress";
                    db.TaxiOffers.Update(taxiOffer);
                    await db.SaveChangesAsync();
                    return View(taxiOffer);
                }
            }
            return NotFound();
        }

        
        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDeleteTaxiOffer(int? id)
        {
            if (id != null)
            {
                TaxiOffer taxiOffer = await db.TaxiOffers.FirstOrDefaultAsync(p => p.Id == id);
                if (taxiOffer != null)
                    return View(taxiOffer);
            }
            return NotFound();
        }

    }
}
