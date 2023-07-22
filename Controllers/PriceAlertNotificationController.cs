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

        /*
         * POST: PriceAlertNotification/Create
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Email,PhoneNumber,PriceThreshold,ListingLink,NotifyOnPriceIncrease,NotifyOnPropertyDelist,NotificationType,Note")] PriceAlertSetupViewModel priceAlert)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", priceAlert);
            }

            // Create new price alert notification.
            var priceAlertNotification = new PriceAlertNotification
            {
                Name = priceAlert.Name,
                Email = priceAlert.Email,
                PhoneNumber = priceAlert.PhoneNumber,
                PriceThreshold = priceAlert.PriceThreshold,
                NotifyOnPriceIncrease = priceAlert.NotifyOnPriceIncrease,
                NotifyOnPropertyDelist = priceAlert.NotifyOnPropertyDelist,
                NotificationType = priceAlert.NotificationType,
                Note = priceAlert.Note,
            };

            // Listing link has additional validation requirements, catch any exceptions
            // and add view model error message.
            try
            {
                priceAlertNotification.ListingLink = priceAlert.ListingLink;
            } catch (Exception) 
            {
                ModelState.AddModelError("ListingLink", "Please enter a valid link. Supported sites are: Rightmove and Purplebricks.");
                return View("Index", priceAlert);
            }

            // Add new notification to database.
            _context.Add(priceAlertNotification);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
