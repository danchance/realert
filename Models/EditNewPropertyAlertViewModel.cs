using System.ComponentModel.DataAnnotations;
using System.Security.Permissions;

namespace Realert.Models
{
    public class EditNewPropertyAlertViewModel
    {
        public string? NotificationName { get; set; }

        public byte NotificationFrequency { get; set; }

        public TargetSite TargetSite { get; set; } 

        public string? PropertyType { get; set; }
        public string? Location { get; set; }
        public float SearchRadius { get; set; }
        public uint MinPrice { get; set; }
        public uint MaxPrice { get; set; }

        private string? _minBeds;
        public string? MinBeds 
        { 
            get { return _minBeds; } 
            set { _minBeds = value ?? "No min"; } 
        }

        private string? _maxBeds;
        public string? MaxBeds
        {
            get { return _maxBeds; }
            set { _maxBeds = value ?? "No max"; }
        }

        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; set; }

        public string? DeleteCode { get; set; }

        public string TargetSiteColorClass
        {
            get
            {
                return TargetSite == TargetSite.Rightmove ? "bg-green-400" : "bg-purple-700";
            }
        }

        public string FormattedPropertyType
        {
            get
            {
                var propertyDictionary = new Dictionary<string, string>
                {
                    {"houses", "Houses"},
                    {"flats", "Flats"},
                    {"commercial", "Commercial Property"},
                    {"land", "Land"},
                    {"bungalows", "Bungalows"},
                    {"other", "Other Property"},
                };
                return PropertyType == null ? "Properties" : propertyDictionary[PropertyType];
            }
        }

        public string MaxPriceString
        {
            get
            {
                if (MaxPrice == 0) return "No max";
                return MaxPrice.ToString();
            }
        }
    }
}
