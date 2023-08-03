using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Realert.Data;
using Realert.Interfaces;
using Realert.Models;
using Realert.Models.ViewModels;

namespace Realert.Controllers
{
    public class PriceAlertNotificationController : Controller
    {
        private readonly RealertContext context;
        private readonly IPriceAlertService priceAlertService;

        public PriceAlertNotificationController(RealertContext context, IPriceAlertService priceAlertService)
        {
            this.context = context;
            this.priceAlertService = priceAlertService;
        }

        /// <summary>
        /// GET: PriceAlertNotification.
        /// </summary>
        public IActionResult Index()
        {
            return this.View("Create", new PriceAlertSetupViewModel());
        }

        /// <summary>
        /// GET: PriceAlertNotification/Create.
        /// </summary>
        public IActionResult Create()
        {
            return this.RedirectToAction(nameof(this.Index));
        }

        /// <summary>
        /// GET: PriceAlertNotification/Created.
        /// </summary>
        /// <param name="successPriceAlert">Details of the added price alert.</param>
        public IActionResult Created(PriceAlertSuccessViewModel successPriceAlert)
        {
            if (successPriceAlert.ContactDetails == null || successPriceAlert.PropertyName == null)
            {
                return this.RedirectToAction(nameof(this.Index));
            }

            return this.View("Created", successPriceAlert);
        }

        /// <summary>
        /// GET PriceAlertNotification/Property/[id].
        /// Displays information about the property linked to the Price Alert.
        /// </summary>
        /// <param name="id">Property Id.</param>
        public async Task<IActionResult> Property(int? id)
        {
            if (id == null || this.context.NewPropertyAlertNotification == null)
            {
                return this.RedirectToAction(nameof(this.Index));
            }

            var priceAlertNotification = await this.context.PriceAlertNotification.Include("Property").FirstOrDefaultAsync(n => n.Id == id);
            if (priceAlertNotification == null)
            {
                return this.RedirectToAction(nameof(this.Index));
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

            return this.View(editPriceAlertViewModel);
        }

        /// <summary>
        /// POST: PriceAlertNotification/Create.
        /// Adds a Price Alert.
        /// </summary>
        /// <param name="priceAlert">Price Alert to add.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Email,PhoneNumber,PriceThreshold,ListingLink,NotifyOnPriceIncrease,NotifyOnPropertyDelist,NotificationType,Note")] PriceAlertSetupViewModel priceAlert)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View("Create", priceAlert);
            }

            // Create new price alert notification.
            PriceAlertNotification priceAlertNotification = new ()
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
            }
            catch (Exception)
            {
                this.ModelState.AddModelError("ListingLink", "Please enter a valid link. Supported sites are: Rightmove and Purplebricks.");
                return this.View("Create", priceAlert);
            }

            // Add the Price Alert.
            int id = await this.priceAlertService.AddAlertAsync(priceAlertNotification);

            // Read the price alert to get the property details for the success message.
            var newPriceAlert = await this.context.PriceAlertNotification.Include("Property").FirstAsync(n => n.Id == id);

            // Setup success view model.
            PriceAlertSuccessViewModel createdPriceAlert = new ()
            {
                ContactDetails = newPriceAlert.NotificationType == Notification.Email ?
                    newPriceAlert.Email :
                    newPriceAlert.PhoneNumber,
                PropertyName = newPriceAlert.Property!.PropertyName,
            };

            return this.RedirectToAction(nameof(this.Created), createdPriceAlert);
        }

        /// <summary>
        /// GET: PriceAlertNotification/Delete/[id].
        /// </summary>
        /// <param name="id">Id of the alert to delete.</param>
        /// <param name="displayError">True if an error occurred deleting the alert.</param>
        public async Task<IActionResult> Delete(int? id, bool? displayError)
        {
            if (id == null || this.context.PriceAlertNotification == null)
            {
                return this.RedirectToAction(nameof(this.Index));
            }

            // Get details for the notification and the associated property.
            var priceAlertNotification = await this.context.PriceAlertNotification.Include("Property").FirstOrDefaultAsync(n => n.Id == id);
            if (priceAlertNotification == null)
            {
                return this.RedirectToAction(nameof(this.Index));
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
                this.ModelState.AddModelError("DeleteCode", "You do not have permission to delete this price alert. Please use the unsubscribe link on an email/text to stop receiving alerts.");
            }

            return this.View(editPriceAlertViewModel);
        }

        /// <summary>
        /// POST: PriceAlertNotification/Delete/[id].
        /// Deletes a Price Alert.
        /// </summary>
        /// <param name="id">Id of the alert to delete.</param>
        /// <param name="editPriceAlert">Details of the alert.</param>
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, [Bind("DeleteCode")] EditPriceAlertViewModel editPriceAlert)
        {
            if (this.context.PriceAlertNotification == null)
            {
                return this.Problem("Entity set 'RealertContext.PriceAlertNotification' is null.");
            }

            // Find the alert with the supplied Id and verify it exists.
            var priceAlertNotification = await this.context.PriceAlertNotification.Include("Property").FirstOrDefaultAsync(n => n.Id == id);
            if (priceAlertNotification == null)
            {
                return this.RedirectToAction(nameof(this.Index));
            }

            // Verify the DeleteCode of the alert matches the DeleteCode supplied by the user,
            // this ensures only the user receiving the emails/texts can delete the alert.
            if (priceAlertNotification.DeleteCode != editPriceAlert.DeleteCode)
            {
                return this.RedirectToAction(nameof(this.Delete), new { id, displayError = true });
            }

            // Setup success view model.
            PriceAlertSuccessViewModel deletedPriceAlert = new ()
            {
                ContactDetails = priceAlertNotification.NotificationType == Notification.Email ?
                    priceAlertNotification.Email :
                    priceAlertNotification.PhoneNumber,
                PropertyName = priceAlertNotification.Property!.PropertyName,
            };

            // Delete the price alert.
            await this.priceAlertService.DeleteAlertAsync(priceAlertNotification);

            return this.View("Deleted", deletedPriceAlert);
        }
    }
}
