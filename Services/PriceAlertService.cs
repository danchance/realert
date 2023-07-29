﻿using Amazon.SimpleEmail;
using Realert.Data;
using Realert.Models;
using Realert.Scrapers;

namespace Realert.Services
{
    public sealed class PriceAlertService
    {
        private readonly RealertContext _context;
        private readonly EmailService _emailService;

        //private readonly ILogger<PriceAlertService> _logger;

        public PriceAlertService(RealertContext context)
        {
            _context = context;

            var host = Host.CreateDefaultBuilder().ConfigureServices((_, services) => services.AddAWSService<IAmazonSimpleEmailService>().AddTransient<EmailService>()).Build();
            _emailService = host.Services.GetRequiredService<EmailService>();

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
         * Used to delete a price alert and the linked property and, if the user has delist alerts on,
         * send a notification to inform them.
         */
        public async Task DeletePriceAlert(PriceAlertNotification priceAlert, bool isDelist = false)
        {
            _context.Remove(priceAlert);
            await _context.SaveChangesAsync();

            // If the property was delisted and user has delist alerts on, send notification.
            if (isDelist && priceAlert.NotifyOnPropertyDelist)
            {
                await SendDelistAlert(priceAlert, priceAlert.Property!);
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

            // If price has decreased always send notifcation.
            // If price has increased, check if user wants to receive price alerts.
            if (priceDifference < 0 || priceAlert.NotifyOnPriceIncrease)
            {
                await SendPriceAlert(priceAlert, property, propertyScraper.PropertyPrice);
            }

            // Update property with the latest price data.
            property.LastScannedPrice = propertyScraper.PropertyPrice;
            _context.Update(property);
            await _context.SaveChangesAsync();
        }

        /*
         * Used to send an email or text notification to the user to notify them that the property
         * price has increased/decreased.
         */
        public async Task SendPriceAlert(PriceAlertNotification priceAlert, PriceAlertProperty property, int newPrice)
        {
            // Send an email notification.
            if (priceAlert.NotificationType == Notification.Email)
            {
                var toAddresses = new List<string> { priceAlert.Email! };
                var subject = $"Realert - Price Change";
                var bodyHtml = $"Hi {priceAlert.Name},<br><br>Good news, the property <a href={priceAlert.ListingLink}>{property.PropertyName}</a> has dropped in price.<br><br>Original Price: £{property.FirstScannedPrice}<br>Current Price: £{newPrice}<br><br><br><p style=\"font-size:12px\">If you'd like to stop receiving these emails you can unsubscribe <a href=\"https://localhost:7231/PriceAlertNotification/Delete/{priceAlert.Id}?code={priceAlert.DeleteCode}\">here</a>.</p>";

                var messageId = await _emailService.SendEmailAsync(toAddresses, bodyHtml, subject); 

                Console.WriteLine(messageId);
                return;
            }

            // Send a text notification.
            // TODO: Use AWS SNS to send text message.
        }

        /*
         * Used to send an email or text notification to the user to notify them that the property has
         * been delisted.
         */
        private async Task SendDelistAlert(PriceAlertNotification priceAlert, PriceAlertProperty property)
        {
            // Send an email notification.
            if (priceAlert.NotificationType == Notification.Email) 
            {
                var toAddresses = new List<string> { priceAlert.Email! };
                var subject = $"Realert - Property Delisted";
                var bodyHtml = $"Hi {priceAlert.Name},<br><br>Bad news, the property <a href={priceAlert.ListingLink}>{property.PropertyName}</a> has been delisted and you will no longer receive alerts for this property.<br><br>Visit us <a href=\"\">here</a> to setup a new alert.<br><br>Thanks, Realert ";

                var messageId = await _emailService.SendEmailAsync(toAddresses, bodyHtml, subject);

                Console.WriteLine(messageId);
                return;
            }

            // Send a text notification.
            // TODO: Use AWS SNS to send text message.
        }

    }
}
