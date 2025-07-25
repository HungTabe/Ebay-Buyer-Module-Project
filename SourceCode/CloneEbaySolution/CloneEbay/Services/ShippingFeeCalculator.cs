namespace CloneEbay.Services
{
    public static class ShippingFeeCalculator
    {
        public static decimal Calculate(string region)
        {
            return region switch
            {
                "Urban" => 20m,
                "Suburban" => 40m,
                "Rural" => 60m,
                _ => 50m
            };
        }
    }
} 