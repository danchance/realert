using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Realert.Data;
using Realert.Models;

namespace Realert.Controllers
{
    public class PriceAlertNotificationController : Controller
    {
        private readonly RealertContext _context;

        public PriceAlertNotificationController(RealertContext context)
        {
            _context = context;
        }

        // GET: PriceAlertNotification
        public IActionResult Index()
        {
            return View(new PriceAlertSetupViewModel());

        }

        // GET: PriceAlertNotification/Create
        public IActionResult Create()
        {
            return View("Index");
        }

        // POST: PriceAlertNotification/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Email,PhoneNumber,PriceThreshold,ListingLink,NotifyOnPriceIncrease,NotifyOnPropertyDelist,NotificationType,Note")] PriceAlertSetupViewModel priceAlert)
        {
            Console.WriteLine(priceAlert.NotifyOnPriceIncrease);
            Console.WriteLine(priceAlert.NotifyOnPropertyDelist);
            if (!ModelState.IsValid)
            {

                return View("Index", priceAlert);
            }
            // Determine target site using the listing link.
            var url = new Uri(priceAlert.ListingLink!);
            var targetSiteDictionary = new Dictionary<string, TargetSite> 
            {
                {"www.rightmove.co.uk", TargetSite.RIGHTMOVE},
                {"www.purplebricks.co.uk", TargetSite.PURPLEBRICKS},
            };
            // Create new price alert property.
            // Create new price alert notification.
            var priceAlertNotification = new PriceAlertNotification
            {
                Name = priceAlert.Name,
                Email = priceAlert.Email,
                PhoneNumber = priceAlert.PhoneNumber,
                ListingLink = priceAlert.ListingLink,
                TargetSite = targetSiteDictionary[url.Host],
                PriceThreshold = priceAlert.PriceThreshold,
                NotifyOnPriceIncrease = priceAlert.NotifyOnPriceIncrease,
                NotifyOnPropertyDelist = priceAlert.NotifyOnPropertyDelist,
                NotificationType = priceAlert.NotificationType,
                Note = priceAlert.Note,
                CreatedAt = DateTime.Today,
                DeleteCode = "123",

            };
            _context.Add(priceAlertNotification);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
           
            //return View("Index", priceAlert);
        }
    
    }
}
