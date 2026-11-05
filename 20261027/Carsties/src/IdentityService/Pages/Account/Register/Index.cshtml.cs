using IdentityService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityService.Pages.Account.Register;

[SecurityHeaders]
[AllowAnonymous]
public class Index(UserManager<ApplicationUser> userManager) : PageModel
{
    [BindProperty] public InputModel Input { get; set; } = null!;
    
    public IActionResult OnGet(string returnUrl)
    {
        Input = new InputModel()
        {
            ReturnUrl = returnUrl
        };
        
        return Page();
    }
}