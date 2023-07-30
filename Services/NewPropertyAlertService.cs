using Amazon.SimpleEmail;
using Realert.Data;
using Realert.Models;
using Realert.Scrapers;
using Realert.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Realert.Services
{
    public class NewPropertyAlertService : IAlertService<NewPropertyAlertNotification>
    {
        private readonly RealertContext _context;
        private readonly EmailService _emailService;

        public NewPropertyAlertService(RealertContext context)
        {
            _context = context;

            var host = Host.CreateDefaultBuilder().ConfigureServices((_, services) => services.AddAWSService<IAmazonSimpleEmailService>().AddTransient<EmailService>()).Build();
            _emailService = host.Services.GetRequiredService<EmailService>();
        }

        /*
         * Add a New Property Alert.
         */
        public async Task AddAlertAsync(NewPropertyAlertNotification newPropertyAlert)
        {
            _context.Add(newPropertyAlert);
            await _context.SaveChangesAsync();
        }

        /*
         * Delete a New Property Alert.
         */
        public async Task DeleteAlertAsync(NewPropertyAlertNotification newPropertyAlert)
        {
            _context.Remove(newPropertyAlert);
            await _context.SaveChangesAsync();
        }

        /*
         * Checks all New Property Alerts setup for any new properties listed. 
         * If new property listings are found a notification is sent to the user.
         */
        public async Task PerformScanAsync()
        {
            var newPropertyAlerts = _context.NewPropertyAlertNotification.ToList();

            for (int i = 0; i < newPropertyAlerts.Count; i++)
            {
                NewPropertyAlertNotification newPropertyAlert = newPropertyAlerts[i];

                // If no scan has been performed before, carry out the first scan
                if (newPropertyAlert.LastScannedDate == null)
                {
                    await ScanNewPropertiesAsync(newPropertyAlert);
                    continue;
                }

                // Scan has already been performed, check if enough time has elapsed 
                // since the last scan.
                int daysSinceLastScan = (DateTime.Today - (DateTime)newPropertyAlert.LastScannedDate!).Days;
                if (daysSinceLastScan >= newPropertyAlert.NotificationFrequency) 
                {
                    await ScanNewPropertiesAsync(newPropertyAlert);
                }
            }
        }

        /*
         * Scans for new properties that match the search query, and notifies the user if
         * it finds any.
         */
        private async Task ScanNewPropertiesAsync(NewPropertyAlertNotification newPropertyAlert)
        {
            string url = newPropertyAlert.GetUrl();

            // Attempt to fetch data, if we get no response exit.
            NewListingsWebScraper propertyScraper;
            try
            {
                propertyScraper = await NewListingsWebScraper.InitializeAsync(url, newPropertyAlert.TargetSite);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); 
                return;
            }

            // Exit if there are no results.
            if (propertyScraper.ResultCount == 0)
            {
                return;
            }

            // Results found, send a notfication the to user.
            await SendNewPropertyAlert(newPropertyAlert, propertyScraper.ResultCount, url);

            // Only update last scanned date if a notification was sent to the user.
            newPropertyAlert.LastScannedDate = DateTime.Today;
            _context.Update(newPropertyAlert);
            await _context.SaveChangesAsync();
        }

        /*
         * Used to send an email notification when new property listings are found.
         */
        private async Task SendNewPropertyAlert(NewPropertyAlertNotification newPropertyAlert, int resultCount, string link)
        {
            var toAddresses = new List<string> { newPropertyAlert.Email! };
            var subject = $"Realert - New Properties Found";
            var bodyHtml = $"<strong>{resultCount}</strong> new properties have been found for your alert: {newPropertyAlert.NotificationName}<br><br>Take a look <a href=\"{link}\">here</a>.<br><br><br><p style=\"font-size:12px\">If you'd like to stop receiving these emails you can unsubscribe <a href=\"https://localhost:7231/NewPropertyAlertNotification/Delete/{newPropertyAlert.Id}?code={newPropertyAlert.DeleteCode}\">here</a>.</p>";

            var messageId = await _emailService.SendEmailAsync(toAddresses, bodyHtml, subject);

            Console.WriteLine(messageId);
        }
    }
}
