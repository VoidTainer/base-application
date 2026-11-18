using BaseApplication.Data;
using BaseApplication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BaseApplication.Pages;

public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _users;

    public IndexModel(AppDbContext db, UserManager<ApplicationUser> users)
    { _db = db; _users = users; }

    public List<Service> Services { get; set; } = new();
    public List<Review> FeaturedReviews { get; set; } = new();

    [BindProperty] public DateTime Date { get; set; } = DateTime.Today.AddDays(1);
    [BindProperty] public TimeSpan StartTime { get; set; } = new(12, 0, 0);
    [BindProperty] public int Guests { get; set; } = 2;
    [BindProperty] public List<int> SelectedServiceIds { get; set; } = new();
    [BindProperty] public string? Comment { get; set; }

    public async Task OnGetAsync()
    {
        await LoadDataAsync();
    }

    public async Task<IActionResult> OnPostBookAsync()
    {
        await LoadDataAsync();

        if (!User.Identity!.IsAuthenticated)
        {
            TempData["Msg"] = "Чтобы забронировать, войдите или зарегистрируйтесь.";
            return RedirectToPage("/Account/Login",
                new { ReturnUrl = Url.Page("/Booking") });
        }

        if (Date.Date < DateTime.Today)
            ModelState.AddModelError("Date", "Дата не может быть в прошлом.");
        if (StartTime < TimeSpan.FromHours(8) || StartTime > TimeSpan.FromHours(22))
            ModelState.AddModelError("StartTime", "Бронирование с 08:00 до 22:00.");
        if (Guests < 1 || Guests > 100)
            ModelState.AddModelError("Guests", "Количество гостей: от 1 до 100.");

        if (!ModelState.IsValid)
        {
            TempData["Msg"] = "Проверьте данные формы бронирования.";
            return Page();
        }

        var user = await _users.GetUserAsync(User);
        if (user == null) return Challenge();

        var validIds = Services.Select(s => s.Id).ToHashSet();

        var booking = new Booking
        {
            UserId = user.Id,
            Date = Date.Date,
            StartTime = StartTime,
            Guests = Guests,
            Comment = Comment,
            Status = BookingStatus.Pending,
            BookingServices = SelectedServiceIds
                .Where(id => validIds.Contains(id))
                .Select(id => new BookingService { ServiceId = id })
                .ToList()
        };

        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync();

        TempData["Msg"] = "Бронирование успешно создано! Проверить статус можно в личном кабинете.";
        return RedirectToPage("/Account/Cabinet");
    }

    private async Task LoadDataAsync()
    {
        Services = await _db.Services
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .Take(6)
            .ToListAsync();

        FeaturedReviews = await _db.Reviews
            .Where(r => r.IsApproved)
            .OrderByDescending(r => r.Rating)
            .ThenByDescending(r => r.CreatedAt)
            .Take(3)
            .ToListAsync();
    }
}