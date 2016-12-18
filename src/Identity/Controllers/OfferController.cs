using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity.Data;
using Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Identity.Controllers
{
    [Authorize]
    public class OfferController : Controller
    {
        private ApplicationDbContext db;
        private readonly ILogger _logger;
        private UserManager<ApplicationUser> userManager;

        private Task<ApplicationUser> GetCurrentUserAsync() => userManager.GetUserAsync(HttpContext.User);
        public OfferController(ILoggerFactory loggerFactory,ApplicationDbContext context, UserManager<ApplicationUser> _userManager)
        {
            db = context;
            userManager = _userManager;
            _logger = loggerFactory.CreateLogger<ManageController>();
        }

        public List<TaxiOffer> GetTaxiOffers()
        {
            List<TaxiOffer> taxiOffers = db.TaxiOffers.ToList();
            List<TaxiOffer> taxi = new List<TaxiOffer>();
            for (int i = 0; i < taxiOffers.Count(); i++)
            {
                if (taxiOffers[i].OfferStatus == "Свободен")
                {
                    taxi.Add(taxiOffers[i]);
                }
            }
            return taxi;
        }
        public JsonResult GetData()
        {
            // создадим список данных
            List<TaxiOffer> taxi = GetTaxiOffers();
            return Json(taxi);
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> TaxiOffers(string searchString, SortState.SortingState sortOrder = SortState.SortingState.StartPointAsc)
        {
            var taxiOffers = await db.TaxiOffers.ToListAsync();
            List<TaxiOffer> taxi = new List<TaxiOffer>();

            for (int i = 0; i < taxiOffers.Count(); i++)
            {
                if (taxiOffers[i].OfferStatus == "Свободен")
                {
                    taxi.Add(taxiOffers[i]);
                }
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                taxi = taxi.Where(s => s.Name.Contains(searchString)).ToList();
            }
            ViewData["PriceSort"] = sortOrder == SortState.SortingState.PriceAsc ? SortState.SortingState.PriceDesc : SortState.SortingState.PriceAsc;

            switch (sortOrder)
            {
                case SortState.SortingState.PriceDesc:
                    taxi = taxi.OrderByDescending(s => s.Price).ToList();
                    break;
                default:
                    taxi = taxi.OrderBy(s => s.Price).ToList();
                    break;
            }

            return View(taxi);
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
            db.Users.Update(user);
            taxiOffer.OfferOwnerId = userId;
            taxiOffer.OfferStatus = "Свободен";
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
                ApplicationUser user = await userManager.FindByIdAsync(taxiOffer.OfferOwnerId);
                String curDateTime =  DateTime.Now.ToString("yyyy-MM-dd") +" "+ DateTime.Now.ToString("HH:mm",
                                         System.Globalization.DateTimeFormatInfo.InvariantInfo);
                _logger.LogInformation(curDateTime);
                if (user.IsAvaliable==null||DateTime.Parse(user.IsAvaliable) < DateTime.Parse(curDateTime))
                {
                    //taxiOffer.OfferStatus = "Выполняется";
                    db.TaxiOffers.Update(taxiOffer);
                    await db.SaveChangesAsync();
                    return View(taxiOffer);
                }
                else
                {
                    ViewData["Avaliable"] = user.IsAvaliable;
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
