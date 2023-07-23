using System.ComponentModel.DataAnnotations;

namespace Realert.Models
{
    public enum Notification
    {
        Email, Text
    }

    public enum TargetSite
    {
        Rightmove, Purplebricks
    }

    /*
     * Model for the Price Alert Nofication table which stores all active
     * email/text notifcations for property prices.
     */
    public class PriceAlertNotification
    {
        public int Id { get; set; }

        /*
         * Users preferred name used when contacting them.
         */
        [Required]
        public string? Name { get; set; }

        /*
         * Notification contact method fields. Email or PhoneNumber is populated
         * depending on the value of NotificationType.
         */
        public Notification NotificationType { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        /*
         * Link to the property listing and the name of the site the listing is on.
         */
        public TargetSite TargetSite { get; private set; }
        private string? _listingLink;
        [Required]
        public string? ListingLink 
        {
            get { return _listingLink; }
            set 
            { 
                if (_listingLink != value)
                {
                    ValidateLink(value!);
                    _listingLink = value;
                }
            } 
        }

        /*
         * Notification options that define the conditions for sending a notification. 
         */
        public int PriceThreshold { get; set; }
        public bool NotifyOnPriceIncrease { get; set; }
        public bool NotifyOnPropertyDelist { get; set; }
        
        /*
         * Misc user notes.
         */
        public string? Note { get; set; }

        /*
         * Date notification was first setup.
         */
        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; private set; }

        /*
         * As there are no user accounts, DeleteCode is used to confirm the user 
         * deleting the notification is the one who is receiving the emails/texts.
         */
        [Required]
        public string DeleteCode { get; private set; }

        /*
         * Details about the property such as price history.
         */
        public virtual PriceAlertProperty? Property { get; set; }

        /*
         * Set values for the CreatedAt timestamp and the DeleteCode.
         */
        public PriceAlertNotification()
        {
            CreatedAt = DateTime.Today;
            DeleteCode = GenerateCode();
        }

        /*
         * Function generates a "random" 10 digit alphanumeric code for the 
         * DeleteCode field. 
         */
        private static string GenerateCode ()
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

        /*
         * Function validates the link to the property listing. If the link is
         * valid the TargetSite field is set. If the link is not a valid link or
         * is not an accepted host an exception is thrown.
         */
        private void ValidateLink (string link)
        {
            // Validate if correctly formatted URI.
            if (!Uri.IsWellFormedUriString (link, UriKind.Absolute))
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
                {"www.rightmove.co.uk", TargetSite.Rightmove},
                {"www.purplebricks.co.uk", TargetSite.Purplebricks},
            };

            // Exception thrown if host does not exist in dictionary.
            TargetSite = targetSiteDictionary[url.Host.ToLower()];
        }
    }
}
