using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        /*
         * GET: PriceAlertNotification/Delete/[id]
         */
        public async Task<IActionResult> Delete(int? id, bool? displayError)
        {
            if (id == null || _context.PriceAlertNotification == null)
            {
                return NotFound();
            }

            // Get details for the notification and the associated property.
            var priceAlertNotification = await _context.PriceAlertNotification.Include("Property").FirstOrDefaultAsync(n => n.Id == id);
            if (priceAlertNotification == null)
            {
                return NotFound();
            }
            var editPriceAlertViewModel = new EditPriceAlertViewModel
            {
                NotificationType = priceAlertNotification.NotificationType,
                TargetSite = priceAlertNotification.TargetSite,
                ListingLink = priceAlertNotification.ListingLink,
                PriceThreshold = priceAlertNotification.PriceThreshold,
                NotifyOnPriceIncrease = priceAlertNotification.NotifyOnPriceIncrease,
                NotifyOnPropertyDelist = priceAlertNotification.NotifyOnPropertyDelist,
                Note = priceAlertNotification.Note,
                CreatedAt = priceAlertNotification.CreatedAt,
                Property = priceAlertNotification.Property,
            };

            // Display an error message if an unauthorized user tried to delete the notification.
            if (displayError != null && displayError == true)
            {
                ModelState.AddModelError("DeleteCode", "You do not have permission to delete this price alert. Please use the unsubscribe link on an email/text to stop receiving alerts.");
            }
            return View(editPriceAlertViewModel);
        }

        /*
         * POST: PriceAlertNotification/Delete/[id]
         */
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, [Bind("NotificationType,DeleteCode, TargetSite,CreatedAt,Note")] EditPriceAlertViewModel editPriceAlert)
        {
            if (_context.PriceAlertNotification == null)
            {
                return Problem("Entity set 'RealertContext.PriceAlertNotification' is null.");
            }

            // Find the alert with the supplied Id and verify it exists.
            var priceAlertNotification = await _context.PriceAlertNotification.FindAsync(id);
            if (priceAlertNotification == null)
            {
                return NotFound();
            }

            // Verify the DeleteCode of the alert matches the DeleteCode supplied by the user, 
            // this ensures only the user receiving the emails/texts can delete the alert.
            if (priceAlertNotification.DeleteCode != editPriceAlert.DeleteCode)
            {            
                return RedirectToAction("Delete", new { id, displayError = true });
            }

            // Delete the price alert.
            _context.PriceAlertNotification.Remove(priceAlertNotification);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
