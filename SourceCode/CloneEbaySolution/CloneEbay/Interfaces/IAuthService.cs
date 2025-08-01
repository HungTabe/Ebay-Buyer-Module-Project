using CloneEbay.Models;
using CloneEbay.Data.Entities;

namespace CloneEbay.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterModel model);
        Task<string?> LoginAsync(LoginModel model);
        Task<bool> SendVerificationEmailAsync(string email);
        Task<User?> GetUserByIdAsync(int userId);
        Task<bool> UpdateProfileAsync(int userId, ProfileUpdateModel model);
    }
}
