using BaseApplication.Data;
using BaseApplication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace BaseApplication.Pages;

public class ContactsModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _users;

    public ContactsModel(AppDbContext db, UserManager<ApplicationUser> users)
    { _db = db; _users = users; }

    [BindProperty] public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required, MaxLength(100)] public string Name { get; set; } = "";
        [Required, EmailAddress] public string Email { get; set; } = "";
        [Required, MaxLength(2000)] public string Message { get; set; } = "";
    }

    public void OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            Input.Name = User.Identity.Name ?? "";
            Input.Email = User.Identity.Name ?? "";
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var userId = User.Identity?.IsAuthenticated == true
            ? (await _users.GetUserAsync(User))?.Id
            : null;

        _db.ContactMessages.Add(new ContactMessage
        {
            UserId = userId,
            Name = Input.Name.Trim(),
            Email = Input.Email.Trim(),
            Message = Input.Message.Trim()
        });
        await _db.SaveChangesAsync();

        TempData["Msg"] = "Спасибо! Ваше сообщение отправлено администратору.";
        return RedirectToPage();
    }
}