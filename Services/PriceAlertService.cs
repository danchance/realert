using Microsoft.EntityFrameworkCore;
using Realert.Data;
using Realert.Interfaces;
using Realert.Models;
using Realert.Scrapers;

namespace Realert.Services
{
    public sealed class PriceAlertService : IPriceAlertService
    {
        // Fields.
        private readonly RealertContext context;
        private readonly IEmailService emailService;
        private readonly ILogger<PriceAlertService> logger;

        public PriceAlertService(RealertContext context, IEmailService emailService, ILogger<PriceAlertService> logger)
        {
            this.context = context;
            this.emailService = emailService;
            this.logger = logger;
        }

        /// <summary>
        /// Method adds a Price Alert and performs a scan for the initial property price.
        /// </summary>
        /// <param name="priceAlert">Alert to add.</param>
        /// <returns>Id of the alert added.</returns>
        public async Task<int> AddAlertAsync(PriceAlertNotification priceAlert)
        {
            // Fetch details of the property.
            // Note: Do this before adding the alert as this will also validate if the
            // property exists/url is valid.
            PropertyListingWebScraper propertyScraper = await PropertyListingWebScraper.InitializeAsync(priceAlert.ListingLink!, priceAlert.TargetSite);

            // Add price alert to database.
            this.context.Add(priceAlert);
            await this.context.SaveChangesAsync();

            // Add property to database.
            await this.AddProperty(propertyScraper.PropertyName!, propertyScraper.PropertyPrice, priceAlert.Id);

            int id = priceAlert.Id;
            this.logger.LogInformation("Price Alert Added, Id = {id}", id);

            return id;
        }

        /// <summary>
        /// Method deletes a Price Alert and the associated property.
        /// </summary>
        /// <param name="priceAlert">Alert to delete.</param>
        /// <returns>Async operation.</returns>
        public async Task DeleteAlertAsync(PriceAlertNotification priceAlert)
        {
            this.context.Remove(priceAlert);
            await this.context.SaveChangesAsync();

            int id = priceAlert.Id;
            this.logger.LogInformation("Price Alert Deleted, Id = {id}", id);
        }

        /// <summary>
        /// Method iterates through all Price Alerts, checking for a change in the property price. If the
        /// price has changed a notification is sent to the user.
        /// </summary>
        /// <returns>Async operation.</returns>
        public async Task PerformScanAsync()
        {
            var priceAlerts = this.context.PriceAlertNotification.Include("Property").ToList();

            for (int i = 0; i < priceAlerts.Count; i++)
            {
                await this.ScanPropertyAsync(priceAlerts[i]);
            }
        }

        /// <summary>
        /// Method adds a property for a Price Alert.
        /// </summary>
        /// <param name="name">Name/address of the property.</param>
        /// <param name="price">Property price.</param>
        /// <param name="alertId">Id of associated alert.</param>
        /// <returns>Async operation.</returns>
        private async Task AddProperty(string name, int price, int alertId)
        {
            // Add property to database.
            PriceAlertProperty priceAlertProperty = new ()
            {
                PropertyName = name,
                FirstScannedPrice = price,
                LastScannedPrice = price,
                PriceAlertNotificationId = alertId,
            };
            this.context.Add(priceAlertProperty);
            await this.context.SaveChangesAsync();
        }

        /// <summary>
        /// Method performs a scan of an individual property, retrieving its current price. If the price
        /// has changed the user is notified according to their preferences.
        /// </summary>
        /// <param name="priceAlert">Alert to perform scan for.</param>
        /// <returns>Async operation.</returns>
        private async Task ScanPropertyAsync(PriceAlertNotification priceAlert)
        {
            PropertyListingWebScraper propertyScraper;
            try
            {
                // Fetch current price data for the property.
                propertyScraper = await PropertyListingWebScraper.InitializeAsync(priceAlert.ListingLink!, priceAlert.TargetSite);
            }
            catch (Exception)
            {
                // Property does not exist, delete the nofication.
                await this.DeleteAlertAsync(priceAlert);

                // If the user has delist alerts on, send notification.
                if (priceAlert.NotifyOnPropertyDelist)
                {
                    var messageId = await this.SendDelistAlertAsync(priceAlert);
                    int id = priceAlert.Id;
                    this.logger.LogInformation("Price Alert Sent [Delist], Id = {id}, MessageId = {messageId}", id, messageId);
                }

                return;
            }

            // Property should not be null at this point, but if it is add it and exit.
            if (priceAlert.Property == null)
            {
                await this.AddProperty(propertyScraper.PropertyName!, propertyScraper.PropertyPrice, priceAlert.Id);
                return;
            }

            // Property is not null, compare current price to the last scan price.
            PriceAlertProperty property = priceAlert.Property;
            int priceDifference = propertyScraper.PropertyPrice - property.LastScannedPrice;

            // If no change in price or the price change is less than the alert threshold, exit.
            if (priceDifference == 0 || Math.Abs(priceDifference) < priceAlert.PriceThreshold)
            {
                return;
            }

            // If price has decreased always send notifcation.
            // If price has increased, check if user wants to receive price alerts.
            if (priceDifference < 0 || priceAlert.NotifyOnPriceIncrease)
            {
                var messageId = await this.SendAlertAsync(priceAlert, propertyScraper.PropertyPrice);
                int id = priceAlert.Id;
                this.logger.LogInformation("Price Alert Sent [Price Change], Id = {id}, MessageId = {messageId}", id, messageId);
            }

            // Update property with the latest price data.
            property.LastScannedPrice = propertyScraper.PropertyPrice;
            this.context.Update(property);
            await this.context.SaveChangesAsync();
        }

        /// <summary>
        /// Method used to send an email or text alert to the user to notify them that the property price
        /// has changed.
        /// </summary>
        /// <param name="priceAlert">Alert to send notification for.</param>
        /// <param name="newPrice">New price of the property.</param>
        /// <returns>Message Id.</returns>
        private async Task<string> SendAlertAsync(PriceAlertNotification priceAlert, int newPrice)
        {
            // Send an email notification.
            if (priceAlert.NotificationType == Notification.Email)
            {
                var toAddresses = new List<string> { priceAlert.Email! };
                var subject = $"Realert - Price Change";
                var bodyHtml = $"Hi {priceAlert.Name},<br><br>Good news, the property <a href=\"https://localhost:7231/PriceAlertNotification/Property/{priceAlert.Id}\">"
                    + $"{priceAlert.Property!.PropertyName}</a> has dropped in price.<br><br>Original Price: £{priceAlert.Property!.FirstScannedPrice}<br>Current Price: "
                    + $"£{newPrice}<br><br><br><p style=\"font-size:12px\">If you'd like to stop receiving these emails you can unsubscribe "
                    + $"<a href=\"https://localhost:7231/PriceAlertNotification/Delete/{priceAlert.Id}?code={priceAlert.DeleteCode}\">here</a>.</p>";

                var messageId = await this.emailService.SendEmailAsync(toAddresses, bodyHtml, subject);

                return messageId;
            }

            // Send a text notification.
            // TODO: Use AWS SNS to send text message.
            return "Text Id: Not yet implemented.";
        }

        /// <summary>
        /// Method used to send an email or text alert to the user to notify them that the property has
        /// been delisted.
        /// </summary>
        /// <param name="priceAlert">Alert for property with price change.</param>
        /// <returns>Message Id.</returns>
        private async Task<string> SendDelistAlertAsync(PriceAlertNotification priceAlert)
        {
            // Send an email notification.
            if (priceAlert.NotificationType == Notification.Email)
            {
                var toAddresses = new List<string> { priceAlert.Email! };
                var subject = $"Realert - Property Delisted";
                var bodyHtml = $"Hi {priceAlert.Name},<br><br>Bad news, the property <a href={priceAlert.ListingLink}>{priceAlert.Property!.PropertyName}</a> has been delisted "
                    + $"and you will no longer receive alerts for this property.<br><br>Visit us <a href=\"\">here</a> to setup a new alert.<br><br>Thanks, Realert";

                var messageId = await this.emailService.SendEmailAsync(toAddresses, bodyHtml, subject);

                return messageId;
            }

            // Send a text notification.
            // TODO: Use AWS SNS to send text message.
            return "";
        }
    }
}
