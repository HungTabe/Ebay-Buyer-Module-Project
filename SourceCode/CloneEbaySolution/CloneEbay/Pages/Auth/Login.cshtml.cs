using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CloneEbay.Models;
using CloneEbay.Services;
using CloneEbay.Interfaces;

namespace CloneEbay.Pages;

public class LoginPageModel : PageModel
{
    private readonly IAuthService _authService;

    public LoginPageModel(IAuthService authService)
    {
        _authService = authService;
    }

    [BindProperty]
    public LoginModel LoginModel { get; set; } = new();

    public string Message { get; set; } = string.Empty;
    public bool IsSuccess { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var token = await _authService.LoginAsync(LoginModel);
            
            if (!string.IsNullOrEmpty(token))
            {
                // Set JWT token in cookie
                Response.Cookies.Append("JWTToken", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(1)
                });

                Message = "Login successful!";
                IsSuccess = true;
                
                // Redirect to home page after successful login
                return RedirectToPage("/Index");
            }
            else
            {
                Message = "Invalid email or password. Please check your credentials.";
                IsSuccess = false;
            }
        }
        catch (Exception ex)
        {
            Message = $"An error occurred: {ex.Message}";
            IsSuccess = false;
        }

        return Page();
    }
} 