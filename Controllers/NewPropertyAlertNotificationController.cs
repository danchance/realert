using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Realert.Data;
using Realert.Models;
using Realert.Interfaces;

namespace Realert.Controllers
{
    public class NewPropertyAlertNotificationController : Controller
    {
        private readonly RealertContext _context;
        private readonly INewPropertyAlertService _newPropertyAlertService;

        public NewPropertyAlertNotificationController(RealertContext context, INewPropertyAlertService newPropertyAlertService)
        {
            _context = context;
            _newPropertyAlertService = newPropertyAlertService;
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
            try
            {
                await _newPropertyAlertService.AddAlertAsync(newPropertyAlertNotification);
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction(nameof(Index));
        }

        /*
         * GET: NewPropertyAlertNotification/Delete/[id]
         */
        public async Task<IActionResult> Delete(int? id, bool? displayError)
        {
            if (id == null || _context.NewPropertyAlertNotification == null)
            {
                return NotFound();
            }

            // Get details for the notification and the associated property.
            var newPropertyAlertNotification = await _context.NewPropertyAlertNotification.FirstOrDefaultAsync(n => n.Id == id);
            if (newPropertyAlertNotification == null)
            {
                return NotFound();
            }

            var editNewPropertyAlertViewModel = new EditNewPropertyAlertViewModel
            {
                NotificationName = newPropertyAlertNotification.NotificationName,
                NotificationFrequency = newPropertyAlertNotification.NotificationFrequency,
                TargetSite = newPropertyAlertNotification.TargetSite,
                PropertyType = newPropertyAlertNotification.PropertyType,
                Location = newPropertyAlertNotification.Location,
                SearchRadius = newPropertyAlertNotification.SearchRadius,
                MinPrice = newPropertyAlertNotification.MinPrice,
                MaxPrice = newPropertyAlertNotification.MaxPrice,
                MinBeds = newPropertyAlertNotification.MinBeds,
                MaxBeds = newPropertyAlertNotification.MaxBeds,
                CreatedAt = newPropertyAlertNotification.CreatedAt,
            };

            // Display an error message if an unauthorized user tried to delete the notification.
            if (displayError != null && displayError == true)
            {
                ModelState.AddModelError("DeleteCode", "You do not have permission to delete this property alert. Please use the unsubscribe link on the email to stop receiving alerts.");
            }
            return View(editNewPropertyAlertViewModel);
        }

        /*
         * POST: NewPropertyAlertNotification/Delete/[id]
         */
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, [Bind("DeleteCode")] EditNewPropertyAlertViewModel editNewPropertyAlert)
        {
            if (_context.NewPropertyAlertNotification == null)
            {
                return Problem("Entity set 'RealertContext.NewPropertyAlertNotification' is null.");
            }

            // Find the alert with the supplied Id and verify it exists.
            var newPropertyAlertNotification = await _context.NewPropertyAlertNotification.FindAsync(id);
            if (newPropertyAlertNotification == null)
            {
                return NotFound();
            }

            // Verify the DeleteCode of the alert matches the DeleteCode supplied by the user, 
            // this ensures only the user receiving the emails can delete the alert.
            if (newPropertyAlertNotification.DeleteCode != editNewPropertyAlert.DeleteCode)
            {
                return RedirectToAction("Delete", new { id, displayError = true });
            }

            // Delete the price alert.
            await _newPropertyAlertService.DeleteAlertAsync(newPropertyAlertNotification);

            return RedirectToAction(nameof(Index));
        }
    }
}
