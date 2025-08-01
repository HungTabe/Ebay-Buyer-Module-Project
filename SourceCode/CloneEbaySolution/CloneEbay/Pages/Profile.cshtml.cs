using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CloneEbay.Interfaces;
using CloneEbay.Models;
using System.Security.Claims;

namespace CloneEbay.Pages
{
    public class ProfileModel : PageModel
    {
        private readonly IAuthService _authService;

        public ProfileModel(IAuthService authService)
        {
            _authService = authService;
        }

        [BindProperty]
        public ProfileUpdateModel ProfileUpdate { get; set; } = new();

        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return RedirectToPage("/Auth/Login");
            }

            var user = await _authService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return RedirectToPage("/Auth/Login");
            }

            // Populate the form with current user data
            ProfileUpdate.Username = user.Username ?? "";
            ProfileUpdate.Email = user.Email ?? "";
            ProfileUpdate.AvatarUrl = user.AvatarUrl;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return RedirectToPage("/Auth/Login");
            }

            var success = await _authService.UpdateProfileAsync(userId, ProfileUpdate);
            if (success)
            {
                Message = "Profile updated successfully!";
                ModelState.Clear();
            }
            else
            {
                ErrorMessage = "Failed to update profile. Email or username might already be taken.";
            }

            return Page();
        }
    }
} 