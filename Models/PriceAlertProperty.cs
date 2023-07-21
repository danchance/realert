﻿namespace Realert.Models
{
    public class PriceAlertProperty
    {
        public int Id { get; set; }
        public string? PropertyName { get; set; }
        public int FirstScannedPrice { get; set; }
        public int LastScannedPrice { get; set; }
        public int NumberOfPriceChanges { get; set; }

        public int PriceAlertNotificationId { get; set; }
        public PriceAlertNotification Notification { get; set; }
    }
}