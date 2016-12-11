using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity.Data;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Identity.Controllers
{
    public class OrderController : Controller
    {


        UserManager<ApplicationUser> userManager;
        private ApplicationDbContext db;
        private readonly ILogger _logger;
        private Task<ApplicationUser> GetCurrentUserAsync() => userManager.GetUserAsync(HttpContext.User);

        public OrderController(ILoggerFactory loggerFactory,ApplicationDbContext context, UserManager<ApplicationUser> _userManager)
        {
            db = context;
            userManager = _userManager;
            _logger = loggerFactory.CreateLogger<ManageController>();
        }

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetData()
        {
            // создадим список данных
            List<Station> stations = new List<Station>();
            stations.Add(new Station()
            {
                Id = 1,
                PlaceName = "Библиотека имени Ленина",
                GeoLat = 37.610489,
                GeoLong = 55.752308,
                Line = "Сокольническая",
                Traffic = 1.0
            });
            stations.Add(new Station()
            {
                Id = 2,
                PlaceName = "Александровский сад",
                GeoLat = 37.608644,
                GeoLong = 55.75226,
                Line = "Арбатско-Покровская",
                Traffic = 1.2
            });
            stations.Add(new Station()
            {
                Id = 3,
                PlaceName = "Боровицкая",
                GeoLat = 37.609073,
                GeoLong = 55.750509,
                Line = "Серпуховско-Тимирязевская",
                Traffic = 1.0
            });

            return Json(stations);
        }

        public async Task<IActionResult> TaxiOrders(SortState.SortingState sortOrder = SortState.SortingState.StartPointAsc)
        {
            IQueryable<TaxiOrder> taxiOrders = db.TaxiOrders.AsNoTracking();
            List<TaxiOrder> taxi = new List<TaxiOrder>();
            for (int i = 0; i < taxiOrders.Count(); i++)
            {
                if ((string)taxiOrders.Skip(i).First().OrderStatus != "Done" && (string)taxiOrders.Skip(i).First().ReceiverId == null)
                {
                    taxi.Add(taxiOrders.Skip(i).First());
                }
            }

            ViewData["StartPointSort"] = sortOrder == SortState.SortingState.StartPointAsc ? SortState.SortingState.StartPointDesc : SortState.SortingState.StartPointAsc;
            ViewData["EndPointSort"] = sortOrder == SortState.SortingState.EndPointAsc ? SortState.SortingState.EndPointDesc : SortState.SortingState.EndPointAsc;
            ViewData["DateSort"] = sortOrder == SortState.SortingState.DateAsc ? SortState.SortingState.DateDesc : SortState.SortingState.DateAsc;
            ViewData["StatusSort"] = sortOrder == SortState.SortingState.StatusOrderAsc ? SortState.SortingState.StatusOrderDesc : SortState.SortingState.StatusOrderAsc;

            switch (sortOrder)
            {
                case SortState.SortingState.StartPointDesc:
                    taxi = taxi.OrderByDescending(s => s.StartPoint).ToList();
                    break;
                case SortState.SortingState.EndPointAsc:
                    taxi = taxi.OrderBy(s => s.EndPoint).ToList();
                    break;
                case SortState.SortingState.EndPointDesc:
                    taxi = taxi.OrderByDescending(s => s.EndPoint).ToList();
                    break;
                case SortState.SortingState.DateAsc:
                    taxi = taxi.OrderBy(s => s.Date).ToList();
                    break;
                case SortState.SortingState.DateDesc:
                    taxi = taxi.OrderByDescending(s => s.Date).ToList();
                    break;
                case SortState.SortingState.StatusOrderAsc:
                    taxi = taxi.OrderBy(s => s.OrderStatus).ToList();
                    break;
                case SortState.SortingState.StatusOrderDesc:
                    taxi = taxi.OrderByDescending(s => s.OrderStatus).ToList();
                    break;
                default:
                    taxi = taxi.OrderBy(s => s.StartPoint).ToList();
                    break;
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

        public async Task<IActionResult> MyRequests()
        {
            var user = await GetCurrentUserAsync();
            var userId = user.Id;
            List<TaxiOrder> allOrders = await db.TaxiOrders.ToListAsync();
            List<TaxiOrder> myRequest = new List<TaxiOrder>();
            foreach (var order in allOrders)
            {
                if (order.ReceiverId == userId)
                {
                    myRequest.Add(order);
                }
            }
            return View(myRequest);
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

        public async Task<IActionResult> CreateTaxiOrderToConcreateDriver(string receiverId)
        {
            TaxiOrder taxiOrder = new TaxiOrder();
            taxiOrder.ReceiverId = receiverId;
            _logger.LogInformation("ReceiverId"+receiverId);
            //db.TaxiOrders.Add(taxiOrder);
           // await db.SaveChangesAsync();
            return View(taxiOrder);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTaxiOrderToConcreateDriver(string receiverId,TaxiOrder taxiOrder)
        {

            var user = await GetCurrentUserAsync();
            var userId = user?.Id;
            taxiOrder.OrderOwnerId = userId;
            taxiOrder.OrderStatus = "In Progress";
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

        
        public async Task<IActionResult> DeleteTaxiOrder(int id)
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
