using Realert.Data;
using Realert.Models;
using Realert.Scrapers;

namespace Realert.Services
{
    public sealed class PriceAlertService
    {
        private readonly RealertContext _context;
        //private readonly ILogger<PriceAlertService> _logger;

        public PriceAlertService(RealertContext context)
        {
            _context = context;
            //_logger = logger;
        }

        /*
         * Performs intial setup for adding a property price alert.
         */
        public async Task AddPriceAlert(PriceAlertNotification priceAlert)
        {
            // Fetch details of the property.
            // Note: Do this before adding the alert as this will also validate if the
            // property exists/url is valid.
            PropertyListingWebScraper propertyScraper = await PropertyListingWebScraper.InitializeAsync(priceAlert.ListingLink!, priceAlert.TargetSite);           

            // Add price alert to database.
            _context.Add(priceAlert);
            await _context.SaveChangesAsync();

            // Add property to database.
            await AddProperty(propertyScraper.PropertyName!, propertyScraper.PropertyPrice, priceAlert.Id);

            return;
        }

        /*
         * Delete a price alert and the linked property.
         */
        public async Task DeletePriceAlert(PriceAlertNotification priceAlert, bool isDelist = false)
        {
            _context.Remove(priceAlert);
            await _context.SaveChangesAsync();

            // If the property was delisted and user has delist alerts on, send notification.
            if (isDelist && priceAlert.NotifyOnPropertyDelist)
            {
                // Notify user
            }
        }

        /*
         * Add property for a price alert.
         */
        private async Task AddProperty(string name, int price, int alertId)
        {
            // Add property to database.
            PriceAlertProperty priceAlertProperty = new()
            {
                PropertyName = name,
                FirstScannedPrice = price,
                LastScannedPrice = price,
                PriceAlertNotificationId = alertId,
            };
            _context.Add(priceAlertProperty);
            await _context.SaveChangesAsync();
        }

        /*
         * Scan property to get its current price, if the price has changed from the 
         * last scan, notify the user.
         */
        public async Task ScanProperty(PriceAlertNotification priceAlert)
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
                await DeletePriceAlert(priceAlert, true);
                return;
            }

            // Property should not be null at this point, but if it is add it and exit.
            if (priceAlert.Property == null)
            {
                await AddProperty(propertyScraper.PropertyName!, propertyScraper.PropertyPrice, priceAlert.Id);
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

            // If price has decreased, send notifcation.
            if (priceDifference < 0)
            {
                // Send notification
            }

            // Price increase, check if user wants to receive price increase alerts.
            if (priceAlert.NotifyOnPriceIncrease)
            {

            }

            // Update property with the latest price data.
            property.LastScannedPrice = propertyScraper.PropertyPrice;
            _context.Update(property);
            await _context.SaveChangesAsync();
        }

    }
}
