using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Realert.Data;
using Realert.Interfaces;
using Realert.Models;

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
            return this.View(new NewPropertyAlertSetupViewModel());
        }

        /// <summary>
        /// GET: NewPropertyAlertNotification/Create.
        /// </summary>
        public IActionResult Create()
        {
            return this.View("Index");
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
                return this.View("Index", newPropertyAlert);
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
                await this.newPropertyAlertService.AddAlertAsync(newPropertyAlertNotification);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return this.RedirectToAction(nameof(this.Index));
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
                return this.NotFound();
            }

            // Get details for the notification and the associated property.
            var newPropertyAlertNotification = await this.context.NewPropertyAlertNotification.FirstOrDefaultAsync(n => n.Id == id);
            if (newPropertyAlertNotification == null)
            {
                return this.NotFound();
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
                return this.NotFound();
            }

            // Verify the DeleteCode of the alert matches the DeleteCode supplied by the user,
            // this ensures only the user receiving the emails can delete the alert.
            if (newPropertyAlertNotification.DeleteCode != editNewPropertyAlert.DeleteCode)
            {
                return this.RedirectToAction("Delete", new { id, displayError = true });
            }

            // Delete the price alert.
            await this.newPropertyAlertService.DeleteAlertAsync(newPropertyAlertNotification);

            return this.RedirectToAction(nameof(this.Index));
        }
    }
}
