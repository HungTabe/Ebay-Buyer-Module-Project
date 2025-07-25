using CloneEbay.Data;
using CloneEbay.Data.Entities;
using CloneEbay.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CloneEbay.Services
{
    public class AddressService : IAddressService
    {
        private readonly CloneEbayDbContext _context;

        public AddressService(CloneEbayDbContext context)
        {
            _context = context;
        }

        public async Task<List<Address>> GetUserAddressesAsync(int userId)
        {
            return await _context.Addresses
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        public async Task<Address?> GetAddressByIdAsync(int addressId, int userId)
        {
            return await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);
        }

        public async Task<bool> AddAddressAsync(Address address)
        {
            try
            {
                // Nếu địa chỉ mới được set làm mặc định, bỏ mặc định các địa chỉ khác
                if (address.IsDefault == true)
                {
                    var existingAddresses = await _context.Addresses
                        .Where(a => a.UserId == address.UserId)
                        .ToListAsync();
                    
                    foreach (var existingAddress in existingAddresses)
                    {
                        existingAddress.IsDefault = false;
                    }
                }

                _context.Addresses.Add(address);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAddressAsync(Address address)
        {
            try
            {
                var existingAddress = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.Id == address.Id && a.UserId == address.UserId);
                
                if (existingAddress == null)
                    return false;

                // Nếu địa chỉ được set làm mặc định, bỏ mặc định các địa chỉ khác
                if (address.IsDefault == true)
                {
                    var otherAddresses = await _context.Addresses
                        .Where(a => a.UserId == address.UserId && a.Id != address.Id)
                        .ToListAsync();
                    
                    foreach (var otherAddress in otherAddresses)
                    {
                        otherAddress.IsDefault = false;
                    }
                }

                // Update properties
                existingAddress.FullName = address.FullName;
                existingAddress.Phone = address.Phone;
                existingAddress.Street = address.Street;
                existingAddress.City = address.City;
                existingAddress.State = address.State;
                existingAddress.Country = address.Country;
                existingAddress.PostalCode = address.PostalCode;
                existingAddress.IsDefault = address.IsDefault;

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAddressAsync(int addressId, int userId)
        {
            try
            {
                var address = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);
                
                if (address == null)
                    return false;

                _context.Addresses.Remove(address);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SetDefaultAddressAsync(int addressId, int userId)
        {
            try
            {
                var address = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);
                
                if (address == null)
                    return false;

                // Bỏ mặc định tất cả địa chỉ của user
                var allAddresses = await _context.Addresses
                    .Where(a => a.UserId == userId)
                    .ToListAsync();
                
                foreach (var addr in allAddresses)
                {
                    addr.IsDefault = false;
                }

                // Set địa chỉ mới làm mặc định
                address.IsDefault = true;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
} 