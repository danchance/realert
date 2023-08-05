using Realert.Data;
using Realert.Interfaces;
using Realert.Models;
using Realert.Scrapers;

namespace Realert.Services
{
    public sealed class NewPropertyAlertService : INewPropertyAlertService
    {
        // Fields.
        private readonly RealertContext context;
        private readonly IEmailService emailService;
        private readonly ILogger<NewPropertyAlertService> logger;

        public NewPropertyAlertService(RealertContext context, IEmailService emailService, ILogger<NewPropertyAlertService> logger)
        {
            this.context = context;
            this.emailService = emailService;
            this.logger = logger;
        }

        /// <summary>
        /// Method adds a New Property Alert.
        /// </summary>
        /// <param name="newPropertyAlert">Alert to add.</param>
        /// <returns>Async operation.</returns>
        public async Task<int> AddAlertAsync(NewPropertyAlertNotification newPropertyAlert)
        {
            this.context.Add(newPropertyAlert);
            await this.context.SaveChangesAsync();

            int id = newPropertyAlert.Id;
            this.logger.LogInformation("New Property Alert Added, Id = {id}", id);

            return id;
        }

        /// <summary>
        /// Method deletes a New Property Alert.
        /// </summary>
        /// <param name="newPropertyAlert">Alert to delete.</param>
        /// <returns>Async operation.</returns>
        public async Task DeleteAlertAsync(NewPropertyAlertNotification newPropertyAlert)
        {
            this.context.Remove(newPropertyAlert);
            await this.context.SaveChangesAsync();

            int id = newPropertyAlert.Id;
            this.logger.LogInformation("New Property Alert Deleted, Id = {id}", id);
        }

        /// <summary>
        /// Method iterates through all New Property Alerts, checking for any new properties listed.
        /// If new property listings are found a notification is sent to the user.
        /// </summary>
        /// <returns>Async operation.</returns>
        public async Task PerformScanAsync()
        {
            var newPropertyAlerts = this.context.NewPropertyAlertNotification.ToList();

            for (int i = 0; i < newPropertyAlerts.Count; i++)
            {
                NewPropertyAlertNotification newPropertyAlert = newPropertyAlerts[i];

                // If no scan has been performed before, carry out the first scan
                if (newPropertyAlert.LastScannedDate == null)
                {
                    await this.ScanNewPropertiesAsync(newPropertyAlert);
                    continue;
                }

                // Scan has already been performed, check if enough time has elapsed
                // since the last scan.
                int daysSinceLastScan = (DateTime.Today - (DateTime)newPropertyAlert.LastScannedDate!).Days;
                if (daysSinceLastScan >= newPropertyAlert.NotificationFrequency)
                {
                    await this.ScanNewPropertiesAsync(newPropertyAlert);
                }
            }
        }

        /// <summary>
        /// Method performs a scan for any new property listings for an individual alert, and notifies
        /// the user if any are found.
        /// </summary>
        /// <param name="newPropertyAlert">Alert to perform scan for.</param>
        /// <returns>Async operation.</returns>
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
            var messageId = await this.SendAlertAsync(newPropertyAlert, propertyScraper.ResultCount, url);
            int id = newPropertyAlert.Id;
            this.logger.LogInformation("New Property Alert Sent, Id = {id}, MessageId = {messageId}", id, messageId);

            // Only update last scanned date if a notification was sent to the user.
            newPropertyAlert.LastScannedDate = DateTime.Today;
            this.context.Update(newPropertyAlert);
            await this.context.SaveChangesAsync();
        }

        /// <summary>
        /// Method used to send an email alert to the usr to notify them that new property listings have been
        /// found.
        /// </summary>
        /// <param name="newPropertyAlert">Alert to send notification for.</param>
        /// <param name="resultCount">Number of new listings found.</param>
        /// <param name="link">Link to the new listings.</param>
        /// <returns>Message Id.</returns>
        private async Task<string> SendAlertAsync(NewPropertyAlertNotification newPropertyAlert, int resultCount, string link)
        {
            var toAddresses = new List<string> { newPropertyAlert.Email! };
            var subject = $"Realert - New Properties Found";
            var bodyHtml = $"<strong>{resultCount}</strong> new properties have been found for your alert: {newPropertyAlert.NotificationName}<br><br>Take a look "
                + $"<a href=\"{link}\">here</a>.<br><br><br><p style=\"font-size:12px\">If you'd like to stop receiving these emails you can unsubscribe "
                + $"<a href=\"https://localhost:7231/NewPropertyAlertNotification/Delete/{newPropertyAlert.Id}?code={newPropertyAlert.DeleteCode}\">here</a>.</p>";

            var messageId = await this.emailService.SendEmailAsync(toAddresses, bodyHtml, subject);

            return messageId;
        }
    }
}
