using Realert.Data;
using Realert.Models;
using Realert.Scrapers;

namespace Realert.Services
{
    public class NewPropertyAlertService
    {
        private readonly RealertContext _context;

        public NewPropertyAlertService(RealertContext context)
        {
            _context = context;
        }

        /*
         * Add a New Property Alert.
         */
        public async Task AddNewPropertyAlert(NewPropertyAlertNotification newPropertyAlert)
        {
            _context.Add(newPropertyAlert);
            await _context.SaveChangesAsync();
        }

        /*
         * Delete a New Property Alert.
         */
        public async Task DeleteNewPropertyAlert(NewPropertyAlertNotification newPropertyAlert)
        {
            _context.Remove(newPropertyAlert);
            await _context.SaveChangesAsync();
        }

        /*
         * Scans for new properties that match the search query, and notifies the user if
         * it finds any.
         */
        public async Task ScanNewProperties(NewPropertyAlertNotification newPropertyAlert)
        {
            string url = newPropertyAlert.GetUrl();

            // Attempt to fetch data, if we get no response exit.
            NewListingsWebScraper propertyScraper;
            try
            {
                propertyScraper = await NewListingsWebScraper.InitializeAsync(url, newPropertyAlert.TargetSite);
            }
            catch (Exception)
            {
                return;
            }

            // Exit if there are no results.
            if (propertyScraper.ResultCount == 0)
            {
                return;
            }

            // Results found, send a notfication the to user.
            

 
        }
    }
}
