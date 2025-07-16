using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CloneEbay.Models;
using CloneEbay.Services;
using CloneEbay.Interfaces;

namespace CloneEbay.Pages;

public class RegisterPageModel : PageModel
{
    private readonly IAuthService _authService;

    public RegisterPageModel(IAuthService authService)
    {
        _authService = authService;
    }

    [BindProperty]
    public RegisterModel RegisterModel { get; set; } = new();

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
            var result = await _authService.RegisterAsync(RegisterModel);
            
            if (result)
            {
                Message = $"Registration successful! Please check your email for verification. Verification link: https://localhost:5000/Auth/VerifyEmail?email={Uri.EscapeDataString(RegisterModel.Email)}";
                IsSuccess = true;
                ModelState.Clear();
                RegisterModel = new RegisterModel();
            }
            else
            {
                Message = "Registration failed. Email or username may already exist.";
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