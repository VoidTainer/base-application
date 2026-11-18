using BaseApplication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace BaseApplication.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly UserManager<ApplicationUser> _users;
    private readonly SignInManager<ApplicationUser> _signIn;
    public RegisterModel(UserManager<ApplicationUser> u, SignInManager<ApplicationUser> s)
    { _users = u; _signIn = s; }

    [BindProperty] public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required, EmailAddress] public string Email { get; set; } = "";
        [Required, MinLength(6)] public string Password { get; set; } = "";
        [Required] public string FullName { get; set; } = "";
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email, FullName = Input.FullName };
        var res = await _users.CreateAsync(user, Input.Password);
        if (!res.Succeeded)
        {
            foreach (var e in res.Errors) ModelState.AddModelError("", e.Description);
            return Page();
        }

        await _users.AddToRoleAsync(user, "User");
        await _signIn.SignInAsync(user, isPersistent: false);
        return RedirectToPage("/Account/Cabinet");
    }
}