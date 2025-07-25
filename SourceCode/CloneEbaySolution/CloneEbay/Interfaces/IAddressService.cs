using CloneEbay.Data.Entities;

namespace CloneEbay.Interfaces
{
    public interface IAddressService
    {
        Task<List<Address>> GetUserAddressesAsync(int userId);
        Task<Address?> GetAddressByIdAsync(int addressId, int userId);
        Task<bool> AddAddressAsync(Address address);
        Task<bool> UpdateAddressAsync(Address address);
        Task<bool> DeleteAddressAsync(int addressId, int userId);
        Task<bool> SetDefaultAddressAsync(int addressId, int userId);
    }
} 