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
        public async Task<string> AddPriceAlert(PriceAlertNotification priceAlert)
        {
            // Fetch details of the property.
            // Note: Do this before adding the alert as this will also validate if the
            // property exists/url is valid.
            PropertyListingWebScraper propertyScraper = await PropertyListingWebScraper.InitializeAsync(priceAlert.ListingLink!, priceAlert.TargetSite);           

            // Add price alert to database.
            _context.Add(priceAlert);
            await _context.SaveChangesAsync();

            // Add property to database.
            PriceAlertProperty priceAlertProperty = new()
            {
                PropertyName = propertyScraper.PropertyName,
                FirstScannedPrice = propertyScraper.PropertyPrice,
                LastScannedPrice = propertyScraper.PropertyPrice,
                PriceAlertNotificationId = priceAlert.Id,
            };
            _context.Add(priceAlertProperty);
            await _context.SaveChangesAsync();

            return "";
        }

        /*
         * Scan property for the current price, if it has changed from the last 
         * scan, notify the user.
         */
        public async Task<string> ScanProperty(PriceAlertNotification priceAlert)
        {
            return "";
        }

    }
}
