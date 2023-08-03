namespace Realert.Models.ViewModels
{
    /// <summary>
    /// View Model <see cref="PriceAlertDeletedViewModel"/> used to display details of
    /// a deleted Price Alert.
    /// </summary>
    public class PriceAlertDeletedViewModel
    {
        /// <value>
        /// Either Email address or phone number depending on the contact method chosen
        /// when setting up the alert.
        /// </value>
        public string? ContactDetails { get; set; }

        /// <value>
        /// The property associated with the notification that has just been deleted.
        /// </value>
        public string? PropertyName { get; set; }
    }
}
