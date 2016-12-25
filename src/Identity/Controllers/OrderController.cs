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

        public IActionResult Error(string receiverId)
        {
            ViewData["Receiver"] = receiverId;
            return View();
        }

        public async Task<IActionResult> TaxiOrders(string searchString,SortState.SortingState sortOrder = SortState.SortingState.StartPointAsc)
        {
            var taxiOrders = await db.Orders.ToListAsync();
            List<Order> taxi = new List<Order>();
           
            for (int i = 0; i < taxiOrders.Count(); i++)
            {
                if (taxiOrders[i].OrderStatus != "Выполнен" && taxiOrders[i].ReceiverId==null && taxiOrders[i].OrderStatus != "Выполняется")
                {
                    taxi.Add(taxiOrders[i]);
                }
            }
            if (!String.IsNullOrEmpty(searchString))
            {
                taxi = taxi.Where(s => s.StartPoint.Contains(searchString)).ToList();
            }
            taxi.OrderByDescending(s => s.StartPoint).ToList();
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
                    taxi = taxi.OrderBy(s => s.Time).ToList();
                    break;
                case SortState.SortingState.DateDesc:
                    taxi = taxi.OrderByDescending(s => s.Time).ToList();
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
            return View(taxi.ToList());
        }

        public async Task<IActionResult> MyOrders()
        {
            var user = await GetCurrentUserAsync();
            var userId = user?.Id;
            List<Order> allOrders = await db.Orders.ToListAsync();
            List<Order> myOrders = new List<Order>();
            foreach (var order in allOrders)
            {
                if (order.OrderOwnerId == userId&& order.OrderStatus != "Выполнен")
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
            List<Order> allOrders = await db.Orders.ToListAsync();
            List<Order> myRequest = new List<Order>();
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
        public async Task<IActionResult> CreateTaxiOrder(Order Order)
        {
      
            var user = await GetCurrentUserAsync();
            var userId = user?.Id;
            Order.OrderOwnerId = userId;
            Order.OrderStatus = "Свободен";
            db.Orders.Add(Order);
            await db.SaveChangesAsync();
            return RedirectToAction("TaxiOrders");
        }

        public async Task<IActionResult> CreateTaxiOrderToConcreateDriver(string receiverId)
        {
            Order Order = new Order();
            Order.ReceiverId = receiverId;
            _logger.LogInformation("ReceiverId"+receiverId);
            //db.Orders.Add(Order);
            //await db.SaveChangesAsync();
            return View(Order);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTaxiOrderToConcreateDriver(string receiverId,Order Order)
        {
            var user = await GetCurrentUserAsync();
            var userId = user?.Id;
            var taxiDriver = await userManager.FindByIdAsync(receiverId);
            _logger.LogInformation(Order.Date + " " + Order.Time);
            _logger.LogInformation(taxiDriver.IsAvaliable);
            String curDateTime = Order.Date+" "+Order.Time;

            if (taxiDriver.IsAvaliable == null || DateTime.Parse(taxiDriver.IsAvaliable) < DateTime.Parse(curDateTime))
            {
                Order.OrderOwnerId = userId;
                Order.OrderStatus = "Cвободен";
                db.Orders.Add(Order);
                await db.SaveChangesAsync();
                return RedirectToAction("TaxiOrders");
            }
            else
            {
                return NotFound("Водитель доступен с "+ taxiDriver.IsAvaliable);
            }

        }

        public async Task<IActionResult> DetailsTaxiOrder(int? id)
        {
            if (id != null)
            {
                Order Order = await db.Orders.FirstOrDefaultAsync(p => p.Id == id);
                if (Order != null)
                    return View(Order);
            }
            return NotFound();
        }

        public async Task<IActionResult> EditTaxiOrder(int? id)
        {
            if (id != null)
            {
                Order Order = await db.Orders.FirstOrDefaultAsync(p => p.Id == id);
                if (Order != null)
                    return View(Order);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> EditTaxiOrder(Order Order)
        {
            var user = await GetCurrentUserAsync();
            var userId = user?.Id;
            Order.OrderOwnerId = userId;
            Order.OrderStatus = "Свободен";
            db.Orders.Update(Order);
            await db.SaveChangesAsync();
            return RedirectToAction("TaxiOrders");
        }

        
        public async Task<IActionResult> DeleteTaxiOrder(int id)
        {
            if (id != null)
            {
                Order Order = await db.Orders.FirstOrDefaultAsync(p => p.Id == id);
                if (Order != null)
                    return View(Order);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTaxiOrder(int? id)
        {
            if (id != null)
            {
                Order Order = await db.Orders.FirstOrDefaultAsync(p => p.Id == id);
                if (Order != null)
                {
                    db.Orders.Remove(Order);
                    await db.SaveChangesAsync();
                    return RedirectToAction("TaxiOrders");
                }
            }
            return NotFound();
        }


        
        public async Task<IActionResult> AgreeTaxiOrder(int id, string expectedTime)
        {
            if (id != null)
            {
                Order Order = await db.Orders.FirstOrDefaultAsync(p => p.Id == id);
                if (Order != null)
                {
                    return View(Order);
                }
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> AgreeTaxiOrder(int? id,string expectedTime,string expectedDate)
        {
            if (id != null)
            {
                Order Order = await db.Orders.FirstOrDefaultAsync(p => p.Id == id);
                
                List<Order> Orders = await db.Orders.ToListAsync();
                for (int i = 0; i < Orders.Count(); i++)
                {
                    _logger.LogInformation(expectedTime);
                    Order.ExpectedTime = expectedTime;
                    Order.ExpectedDate = expectedDate;
                    db.Orders.Update(Order);
                    String curDateTime = Orders[i].Date + " " + Orders[i].Time;
                    _logger.LogInformation("CurD" + curDateTime);
                    String expectedDateTime = expectedDate + " " + expectedTime;
                    _logger.LogInformation("ExpD" + expectedDateTime);
                    if (DateTime.Parse(curDateTime)<DateTime.Parse(expectedDateTime))
                    {
                        //AutoRejectOrder(i);
                        Orders[i].ReceiverId = null;
                        db.Orders.Update(Orders[i]);
                    }
                }

                if (Order != null)
                {
                    var user = await GetCurrentUserAsync();
                    var userId = user?.Id;
                    Order.OrderStatus = "Выполняется";
                    Order.ReceiverId = userId;
                    user.IsAvaliable = expectedDate + " " + expectedTime;
                    db.Users.Update(user);
                    db.Orders.Update(Order);
                    await db.SaveChangesAsync();
                    
                    return View(Order);
                }
            }
            return NotFound();
        }

        public async void AutoRejectOrder(int id)
        {
            List<Order> allOrders = await db.Orders.ToListAsync();
            allOrders[id].ReceiverId = null;
            db.Orders.Update(allOrders[id]);
            await db.SaveChangesAsync();
        }

        public async Task<IActionResult> AgreeTaxiOffer(int? id)
        {
            if (id != null)
            {
                TaxiOffer taxiOffer = await db.TaxiOffers.FirstOrDefaultAsync(p => p.Id == id);
                if (taxiOffer != null)
                {
                    //taxiOffer.OfferStatus = "Выполняется";
                    db.TaxiOffers.Update(taxiOffer);
                    await db.SaveChangesAsync();
                    return View(taxiOffer);
                }
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmTaxiOrder(int? id, Order taxi)
        {
            if (id != null)
            {
                ApplicationUser user = await GetCurrentUserAsync();
                Order Order = await db.Orders.FirstOrDefaultAsync(p => p.Id == id);
                if (Order != null)
                {
                    Order.OrderStatus = "Выполнен";
                    Order.Distanse = taxi.Distanse;
                    Order.Duration = taxi.Duration;
                    _logger.LogInformation(Order.OrderStatus);
                    user.IsAvaliable = null;
                    db.Orders.Update(Order);
                    await db.SaveChangesAsync();
                    return RedirectToAction("TaxiOrders");
                }
            }
            return NotFound();
        }

        public async Task<IActionResult> ConfirmTaxiOrder(int? id)
        {
            Order Order = await db.Orders.FirstOrDefaultAsync(p => p.Id == id);
            return View(Order);
        }

        [HttpGet]
        [ActionName("Delete")]
        public async Task<IActionResult> ConfirmDeleteTaxiOrder(int? id)
        {
            if (id != null)
            {
                Order Order = await db.Orders.FirstOrDefaultAsync(p => p.Id == id);
                if (Order != null)
                    return View(Order);
            }
            return NotFound();
        }


        public async Task<IActionResult> RejectOrder(int? id)
        {
            var user = await GetCurrentUserAsync();
            var userId = user.Id;
            List<Order> allOrders = await db.Orders.ToListAsync();
            foreach (var order in allOrders)
            {
                if (order.ReceiverId == userId&&order.Id==id&&order.OrderStatus!="Выполнен")
                {
                    order.ReceiverId = null;
                    db.Orders.Update(order);
                    await db.SaveChangesAsync();
                }
            }


            return RedirectToAction("MyRequests");
        }

        public async Task<IActionResult> Layout()
        {
            
            return View();
        }
    }
}
