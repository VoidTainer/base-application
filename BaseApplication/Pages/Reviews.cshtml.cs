using BaseApplication.Data;
using BaseApplication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BaseApplication.Pages;

public class ReviewsModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _users;

    public ReviewsModel(AppDbContext db, UserManager<ApplicationUser> users)
    { _db = db; _users = users; }

    public List<Review> Items { get; set; } = new();

    [BindProperty] public int Rating { get; set; } = 5;
    [BindProperty] public string Text { get; set; } = "";

    public async Task OnGetAsync()
    {
        Items = await _db.Reviews
            .Where(r => r.IsApproved)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!User.Identity!.IsAuthenticated)
            return RedirectToPage("/Account/Login", new { ReturnUrl = Url.Page("/Reviews") });

        if (string.IsNullOrWhiteSpace(Text) || Rating < 1 || Rating > 5)
        {
            ModelState.AddModelError("", "Укажите текст отзыва и оценку от 1 до 5.");
            await OnGetAsync();
            return Page();
        }

        var user = await _users.GetUserAsync(User);
        if (user == null) return Challenge();

        _db.Reviews.Add(new Review
        {
            UserId = user.Id,
            AuthorName = string.IsNullOrWhiteSpace(user.FullName) ? (user.Email ?? "Гость") : user.FullName,
            Rating = Rating,
            Text = Text.Trim(),
            IsApproved = false
        });
        await _db.SaveChangesAsync();

        TempData["Msg"] = "Спасибо! Отзыв отправлен на модерацию и появится после проверки.";
        return RedirectToPage();
    }
}