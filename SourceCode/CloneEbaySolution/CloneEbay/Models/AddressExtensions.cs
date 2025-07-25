using CloneEbay.Data.Entities;

namespace CloneEbay.Models
{
    public static class AddressExtensions
    {
        public static string GetFullAddress(this Address address)
        {
            return $"{address.Street}, {address.City}, {address.State}, {address.Country}, {address.PostalCode}";
        }

        public static string GetRegion(this Address address)
        {
            if (address.City != null && (address.City.Contains("Hà Nội") || address.City.Contains("Hồ Chí Minh") || address.City.Contains("HCM")))
                return "Urban";
            return "Suburban";
        }
    }
} 