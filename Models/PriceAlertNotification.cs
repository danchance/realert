using System.ComponentModel.DataAnnotations;

namespace Realert.Models
{
    /// <summary>
    /// Type of notification to send.
    /// </summary>
    public enum Notification
    {
        Email,
        Text,
    }

    /// <summary>
    /// Site the notification is setup for.
    /// </summary>
    public enum TargetSite
    {
        Rightmove,
        Purplebricks,
    }

    /// <summary>
    /// Model <see cref="PriceAlertNotification"/> holds details of all active Price Alerts.
    /// </summary>
    public class PriceAlertNotification
    {
        // Fields.
        private string? listingLink;

        /// <summary>
        /// Initializes a new instance of the <see cref="PriceAlertNotification"/> class.
        /// Set values for the CreatedAt timestamp and the DeleteCode.
        /// </summary>
        public PriceAlertNotification()
        {
            this.CreatedAt = DateTime.Today;
            this.DeleteCode = GenerateCode();
        }

        // Properties.

        /// <value>
        /// Unique Id.
        /// </value>
        public int Id { get; set; }

        /// <value>
        /// Users preferred name to use when contacting them.
        /// </value>
        [Required]
        public string? Name { get; set; }

        /// <value>
        /// Type of notification to send, either email or text.
        /// </value>
        public Notification NotificationType { get; set; }

        /// <value>
        /// Email address, populated if NotificationType = Email.
        /// </value>
        public string? Email { get; set; }

        /// <value>
        /// Email address, populated if NotificationType = Text.
        /// </value>
        public string? PhoneNumber { get; set; }

        /// <value>
        /// The real estate site the property listing is on.
        /// </value>
        public TargetSite TargetSite { get; private set; }

        /// <value>
        /// Link to the property listing the alert is for.
        /// </value>
        [Required]
        public string? ListingLink
        {
            get
            {
                return this.listingLink;
            }

            set
            {
                if (this.listingLink != value)
                {
                    this.ValidateLink(value!);
                    this.listingLink = value;
                }
            }
        }

        /// <value>
        /// The minimum amount the price must change before a notification is sent.
        /// </value>
        public int PriceThreshold { get; set; }

        /// <value>
        /// Determines if the user should be notified if the property price increases.
        /// </value>
        public bool NotifyOnPriceIncrease { get; set; }

        /// <value>
        /// Determines if the user should be notified when the property is delisted.
        /// </value>
        public bool NotifyOnPropertyDelist { get; set; }

        /// <value>
        /// User note containing any details about the notification.
        /// </value>
        public string? Note { get; set; }

        /// <value>
        /// Date the notification was setup.
        /// </value>
        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; private set; }

        /// <value>
        /// Code used to verify that only the user receiving the emails can delete and/or edit
        /// the New Property notification. Needed as there are no user accounts.
        /// </value>
        [Required]
        public string DeleteCode { get; private set; }

        /// <value>
        /// Property the price alert is for.
        /// </value>
        public virtual PriceAlertProperty? Property { get; set; }

        /// <summary>
        /// Method generates a "random" 10 digit alphanumeric code.
        /// </summary>
        /// <returns>10 character "random" string.</returns>
        private static string GenerateCode()
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[10];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(stringChars);
        }

        /// <summary>
        /// Method validates the link to the property listing. If the link is valid the TargetSite
        /// field is set. If the link is not a valid link or is not an accepted site an exception is
        /// thrown.
        /// </summary>
        /// <param name="link">Link to validate.</param>
        /// <exception cref="ArgumentException">Thrown for invalid or unsupported link.</exception>
        private void ValidateLink(string link)
        {
            // Validate if correctly formatted URI.
            if (!Uri.IsWellFormedUriString(link, UriKind.Absolute))
            {
                throw new ArgumentException("Not a valid url");
            }

            var url = new Uri(link);

            // Vaildate url is using HTTPS.
            if (url.Scheme != Uri.UriSchemeHttps)
            {
                throw new ArgumentException("Not a valid url");
            }

            var targetSiteDictionary = new Dictionary<string, TargetSite>
            {
                { "www.rightmove.co.uk", TargetSite.Rightmove },
                { "www.purplebricks.co.uk", TargetSite.Purplebricks },
            };

            // Exception thrown if host does not exist in dictionary.
            this.TargetSite = targetSiteDictionary[url.Host.ToLower()];
        }
    }
}
