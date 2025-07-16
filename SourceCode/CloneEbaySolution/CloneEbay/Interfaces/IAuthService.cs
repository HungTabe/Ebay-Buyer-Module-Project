using CloneEbay.Models;

namespace CloneEbay.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterModel model);
        Task<string?> LoginAsync(LoginModel model);
        Task<bool> SendVerificationEmailAsync(string email);
    }
}
