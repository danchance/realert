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
    public class NewPropertyAlertNotificationController : Controller
    {
        private readonly RealertContext _context;

        public NewPropertyAlertNotificationController(RealertContext context)
        {
            _context = context;
        }

        // GET: NewPropertyAlertNotification
        public IActionResult Index()
        {
            return View(new NewPropertyAlertSetupViewModel());
        }


        // GET: NewPropertyAlertNotification/Create
        public IActionResult Create()
        {
            return View("Index");
        }

        /*
         * POST: NewPropertyAlertNotification/Create
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Email,NotificationName,TargetSite,NotificationFrequency,PropertyType,Location,SearchRadius,MinPrice,MaxPrice,MinBeds,MaxBeds,SearchLink")] NewPropertyAlertSetupViewModel newPropertyAlert)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", newPropertyAlert);
            }

            // Create new property alert notification.
            var newPropertyAlertNotification = new NewPropertyAlertNotification
            {
                Email = newPropertyAlert.Email,
                NotificationName = newPropertyAlert.NotificationName,
                TargetSite = newPropertyAlert.TargetSite,
                NotificationFrequency = newPropertyAlert.NotificationFrequency,
                PropertyType = newPropertyAlert.PropertyType,
                Location = newPropertyAlert.Location,
                SearchRadius = newPropertyAlert.SearchRadius,
                MinPrice = newPropertyAlert.MinPrice,
                MaxPrice = newPropertyAlert.MaxPrice,
                MinBeds = newPropertyAlert.MinBeds,
                MaxBeds = newPropertyAlert.MaxBeds,
            };

            // Add new notification to the database.
            _context.Add(newPropertyAlertNotification);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
