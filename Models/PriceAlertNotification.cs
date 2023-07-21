namespace Realert.Models
{
    public enum Notification
    {
        EMAIL, PHONE
    }

    public enum TargetSite
    {
        RIGHTMOVE, PURPLEBRICKS
    }

    public class PriceAlertNotification
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ListingLink { get; set; }
        public TargetSite TargetSite { get; set; }
        public int PriceThreshold { get; set; }
        public bool NotifyOnPriceIncrease { get; set; }
        public bool NotifyOnPropertyDelist { get; set; }
        public Notification NotificationType { get; set; }
        public string? Note { get; set; }

        public PriceAlertProperty? Property { get; set; }
    }
}
