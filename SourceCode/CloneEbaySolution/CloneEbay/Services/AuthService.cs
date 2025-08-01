using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CloneEbay.Data;
using CloneEbay.Data.Entities;
using CloneEbay.Models;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using CloneEbay.Interfaces;

namespace CloneEbay.Services;

public class AuthService : IAuthService
{
    private readonly CloneEbayDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(CloneEbayDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<bool> RegisterAsync(RegisterModel model)
    {
        // Check if email already exists
        if (_context.Users.Any(u => u.Email == model.Email))
        {
            return false;
        }

        // Check if username already exists
        if (_context.Users.Any(u => u.Username == model.Username))
        {
            return false;
        }

        // Hash password
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

        // Create new user
        var user = new User
        {
            Email = model.Email,
            Username = model.Username,
            Password = hashedPassword,
            Role = "User",
            IsVerified = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Send verification email (simulated)
        await SendVerificationEmailAsync(model.Email);

        return true;
    }

    public async Task<string?> LoginAsync(LoginModel model)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

        if (user == null)
        {
            return null;
        }

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
        {
            return null;
        }

        // Check if user is verified
        if (!user.IsVerified.GetValueOrDefault())
        {
            return null;
        }

        // Generate JWT token
        return GenerateJwtToken(user);
    }

    public async Task<bool> SendVerificationEmailAsync(string email)
    {
        // Simulate email sending
        Console.WriteLine("=".PadRight(50, '='));
        Console.WriteLine(" VERIFICATION EMAIL SENT");
        Console.WriteLine("=".PadRight(50, '='));
        Console.WriteLine($" To: {email}");
        Console.WriteLine($" Verification Link: https://localhost:5000/Auth/VerifyEmail?email={Uri.EscapeDataString(email)}");
        Console.WriteLine(" Please check your email and click the verification link.");
        Console.WriteLine("=".PadRight(50, '='));
        
        // Also write to debug output for better visibility
        System.Diagnostics.Debug.WriteLine($"Verification email sent to: {email}");
        System.Diagnostics.Debug.WriteLine($"Verification link: https://localhost:5000/Auth/VerifyEmail?email={Uri.EscapeDataString(email)}");
        
        // In a real application, you would send an actual email here
        // For now, we'll just simulate the verification process
        await Task.Delay(100); // Simulate async operation
        
        return true;
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Name, user.Username!),
            new Claim(ClaimTypes.Role, user.Role!)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(Convert.ToDouble(jwtSettings["ExpiryInDays"])),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<bool> UpdateProfileAsync(int userId, ProfileUpdateModel model)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null)
        {
            return false;
        }

        // Check if email is already taken by another user
        if (await _context.Users.AnyAsync(u => u.Email == model.Email && u.Id != userId))
        {
            return false;
        }

        // Check if username is already taken by another user
        if (await _context.Users.AnyAsync(u => u.Username == model.Username && u.Id != userId))
        {
            return false;
        }

        // Update user information
        user.Username = model.Username;
        user.Email = model.Email;
        user.AvatarUrl = model.AvatarUrl;

        // Update password if provided
        if (!string.IsNullOrEmpty(model.NewPassword))
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
        }

        await _context.SaveChangesAsync();
        return true;
    }
} 