using Amazon.SimpleEmail;
using Realert.Data;
using Realert.Models;
using Realert.Scrapers;

namespace Realert.Services
{
    public class NewPropertyAlertService
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
            await SendNewPropertyAlert(newPropertyAlert, propertyScraper.ResultCount, url);
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
