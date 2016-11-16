﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity.Data;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Identity.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db;
        private UserManager<ApplicationUser> userManager;
        
        public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> _userManager)
        {
            db = context;
            userManager = _userManager;
        }
        private Task<ApplicationUser> GetCurrentUserAsync() => userManager.GetUserAsync(HttpContext.User);
        public IActionResult Index()
        {
            return View();
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> TaxiOrders()
        {
            return View(await db.TaxiOrders.ToListAsync());
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

        public IActionResult CreateTaxiOrder()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateTaxiOrder(TaxiOrder taxiOrder)
        {

            var user = await GetCurrentUserAsync();
            var userId = user?.Id;
            taxiOrder.OrderOwnerId=userId;
            db.TaxiOrders.Add(taxiOrder);
            await db.SaveChangesAsync();
            return RedirectToAction("TaxiOrders");
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
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
        public async Task<IActionResult> EditTaxiOffer(TaxiOffer taxiOffer)
        {
            db.TaxiOffers.Update(taxiOffer);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> EditTaxiOrder(TaxiOrder taxiOrder)
        {
            db.TaxiOrders.Update(taxiOrder);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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

        [HttpPost]
        public async Task<IActionResult> DeleteTaxiOffer(int? id)
        {
            if (id != null)
            {
                TaxiOffer phone = await db.TaxiOffers.FirstOrDefaultAsync(p => p.Id == id);
                if (phone != null)
                {
                    db.TaxiOffers.Remove(phone);
                    await db.SaveChangesAsync();
                    return RedirectToAction("TaxiOffers");
                }
            }
            return NotFound();
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
    }
}
