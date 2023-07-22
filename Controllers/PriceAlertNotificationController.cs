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
        public async Task<IActionResult> Create([Bind("Name,Email,PriceThreshold,ListingLink,NotifyOnPriceIncrease,NotifyOnPropertyDelist,NotificationType,Note")] PriceAlertSetupViewModel priceAlert)
        {
            Console.WriteLine("here");
            if (ModelState.IsValid)
            {
                //_context.Add(priceAlertNotification);
                //await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
            }
            return View("Index", priceAlert);
        }
    
    }
}
