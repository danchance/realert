using System.ComponentModel.DataAnnotations;

namespace Realert.Models
{
    /*
     * Model for the New Property Alert Nofication table which stores all active
     * email notifcations for new property alerts.
     */
    public class NewPropertyAlertNotification
    {
        [Required]
        [StringLength(100)]
        public string? Email { get; set; }

        /*
         * Name the user gave the notification.
         */
        [Required]
        [StringLength(100)]
        public string? NotificationName { get; set; }

        /*
         * How frequently a scan will occur to check for new properties.
         */
        public byte NotificationFrequency { get; set; }

        /*
         * The site to search for properties on
         */
        public TargetSite TargetSite { get; set; }

        /*
         * Property search parameters that define the type of property to search
         * for.
         */
        public string? PropertyType { get; set; }
        [Required]
        public string? Location { get; set; }
        public float SearchRadius { get; set; }
        public uint MinPrice { get; set; }
        public uint MaxPrice { get; set; }
        [Required]
        public string? MinBeds { get; set; }
        [Required]
        public string? MaxBeds { get; set; }

        /*
         * Date notification was first setup.
         */
        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; private set; }

        /*
         * As there are no user accounts, DeleteCode is used to confirm the user 
         * deleting the notification is the one who is receiving the emails.
         */
        [Required]
        public string DeleteCode { get; private set; }

        /*
         * Set values for the CreatedAt timestamp and the DeleteCode.
         */
        public NewPropertyAlertNotification() 
        {
            CreatedAt = DateTime.Today;
            DeleteCode = GenerateCode();
        }

        /*
         * Function generates a "random" 10 digit alphanumeric code for the 
         * DeleteCode field. 
         */
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
    }
}
