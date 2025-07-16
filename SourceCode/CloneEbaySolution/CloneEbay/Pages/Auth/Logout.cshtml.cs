using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CloneEbay.Pages;

public class LogoutModel : PageModel
{
    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        // Remove JWT token from cookie
        Response.Cookies.Delete("JWTToken");
        
        // Redirect to home page
        return RedirectToPage("/Index");
    }
} 