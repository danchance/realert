﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Realert.Data;
using Realert.Interfaces;
using Realert.Models;
using Realert.Models.ViewModels;

namespace Realert.Controllers
{
    public class NewPropertyAlertNotificationController : Controller
    {
        private readonly RealertContext context;
        private readonly INewPropertyAlertService newPropertyAlertService;

        public NewPropertyAlertNotificationController(RealertContext context, INewPropertyAlertService newPropertyAlertService)
        {
            this.context = context;
            this.newPropertyAlertService = newPropertyAlertService;
        }

        /// <summary>
        /// GET: NewPropertyAlertNotification.
        /// </summary>
        public IActionResult Index()
        {
            return this.View("Create", new NewPropertyAlertSetupViewModel());
        }

        /// <summary>
        /// GET: NewPropertyAlertNotification/Create.
        /// </summary>
        public IActionResult Create()
        {
            return this.RedirectToAction(nameof(this.Index));
        }

        /// <summary>
        /// GET: NewPropertyAlertNotification/Created.
        /// </summary>
        /// <param name="successNewPropertyAlert">Details of the added new property alert.</param>
        public IActionResult Created(NewPropertyAlertSuccessViewModel successNewPropertyAlert)
        {
            if (successNewPropertyAlert.Email == null || successNewPropertyAlert.NotificationName == null)
            {
                return this.RedirectToAction(nameof(this.Index));
            }

            return this.View("Created", successNewPropertyAlert);
        }

        /// <summary>
        /// POST: NewPropertyAlertNotification/Create.
        /// Adds a New Property Alert.
        /// </summary>
        /// <param name="newPropertyAlert">Property Alert to add.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Email,NotificationName,TargetSite,NotificationFrequency,PropertyType,Location,SearchRadius,MinPrice,MaxPrice,MinBeds,MaxBeds,SearchLink")] NewPropertyAlertSetupViewModel newPropertyAlert)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View("Create", newPropertyAlert);
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

            // Setup success view model.
            NewPropertyAlertSuccessViewModel createdNewPropertyAlert = new ()
            {
                NotificationName = newPropertyAlertNotification.NotificationName,
                Email = newPropertyAlertNotification.Email,
            };

            // Add the New Property Alert.
            await this.newPropertyAlertService.AddAlertAsync(newPropertyAlertNotification);

            return this.RedirectToAction(nameof(this.Created), createdNewPropertyAlert);
        }

        /// <summary>
        /// GET: NewPropertyAlertNotification/Delete/[id].
        /// </summary>
        /// <param name="id">Id of the alert to delete.</param>
        /// <param name="displayError">True if an error occurred deleting the alert.</param>
        public async Task<IActionResult> Delete(int? id, bool? displayError)
        {
            if (id == null || this.context.NewPropertyAlertNotification == null)
            {
                return this.RedirectToAction(nameof(this.Index));
            }

            // Get details for the notification and the associated property.
            var newPropertyAlertNotification = await this.context.NewPropertyAlertNotification.FirstOrDefaultAsync(n => n.Id == id);
            if (newPropertyAlertNotification == null)
            {
                return this.RedirectToAction(nameof(this.Index));
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
                this.ModelState.AddModelError("DeleteCode", "You do not have permission to delete this property alert. Please use the unsubscribe link on the email to stop receiving alerts.");
            }

            return this.View(editNewPropertyAlertViewModel);
        }

        /// <summary>
        /// POST: NewPropertyAlertNotification/Delete/[id].
        /// Deletes a New Property Alert.
        /// </summary>
        /// <param name="id">Id of the alert to delete.</param>
        /// <param name="editNewPropertyAlert">Details of the alert.</param>
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, [Bind("DeleteCode")] EditNewPropertyAlertViewModel editNewPropertyAlert)
        {
            if (this.context.NewPropertyAlertNotification == null)
            {
                return this.Problem("Entity set 'RealertContext.NewPropertyAlertNotification' is null.");
            }

            // Find the alert with the supplied Id and verify it exists.
            var newPropertyAlertNotification = await this.context.NewPropertyAlertNotification.FindAsync(id);
            if (newPropertyAlertNotification == null)
            {
                return this.RedirectToAction(nameof(this.Index));
            }

            // Verify the DeleteCode of the alert matches the DeleteCode supplied by the user,
            // this ensures only the user receiving the emails can delete the alert.
            if (newPropertyAlertNotification.DeleteCode != editNewPropertyAlert.DeleteCode)
            {
                return this.RedirectToAction(nameof(this.Delete), new { id, displayError = true });
            }

            // Setup success view model.
            NewPropertyAlertSuccessViewModel deletedNewPropertyAlert = new ()
            {
                NotificationName = newPropertyAlertNotification.NotificationName,
            };

            // Delete the price alert.
            await this.newPropertyAlertService.DeleteAlertAsync(newPropertyAlertNotification);

            return this.View("Deleted", deletedNewPropertyAlert);
        }
    }
}
