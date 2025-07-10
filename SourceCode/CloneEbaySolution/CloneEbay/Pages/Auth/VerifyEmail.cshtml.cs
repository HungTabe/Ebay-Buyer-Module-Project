using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CloneEbay.Data;
using Microsoft.EntityFrameworkCore;

namespace CloneEbay.Pages;

public class VerifyEmailModel : PageModel
{
    private readonly CloneEbayDbContext _context;

    public VerifyEmailModel(CloneEbayDbContext context)
    {
        _context = context;
    }

    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            IsSuccess = false;
            Message = "Invalid verification link.";
            return Page();
        }

        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            
            if (user == null)
            {
                IsSuccess = false;
                Message = "User not found.";
                return Page();
            }

            if (user.IsVerified.GetValueOrDefault())
            {
                IsSuccess = true;
                Message = "Email is already verified.";
                return Page();
            }

            // Mark user as verified
            user.IsVerified = true;
            await _context.SaveChangesAsync();

            IsSuccess = true;
            Message = "Email verified successfully!";
        }
        catch (Exception ex)
        {
            IsSuccess = false;
            Message = $"An error occurred: {ex.Message}";
        }

        return Page();
    }
} 