using CloneEbay.Models;

namespace CloneEbay.Models
{
    public static class AddressViewModelExtensions
    {
        public static string GetRegion(this AddressViewModel address)
        {
            if (address.City != null && (address.City.Contains("Hà Nội") || address.City.Contains("Hồ Chí Minh") || address.City.Contains("HCM")))
                return "Urban";
            return "Suburban";
        }
    }
} 