using BaseApplication.Data;
using BaseApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BaseApplication.Pages;

[Authorize]
public class BookingModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _users;

    public BookingModel(AppDbContext db, UserManager<ApplicationUser> users)
    { _db = db; _users = users; }

    public List<Service> AllServices { get; set; } = new();

    [BindProperty] public DateTime Date { get; set; } = DateTime.Today.AddDays(1);
    [BindProperty] public TimeSpan StartTime { get; set; } = new(12, 0, 0);
    [BindProperty] public int Guests { get; set; } = 2;
    [BindProperty] public List<int> SelectedServiceIds { get; set; } = new();
    [BindProperty] public string? Comment { get; set; }

    public async Task OnGetAsync()
    {
        AllServices = await _db.Services.Where(s => s.IsActive).ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        AllServices = await _db.Services.Where(s => s.IsActive).ToListAsync();

        if (Date.Date < DateTime.Today)
            ModelState.AddModelError(nameof(Date), "Дата не может быть в прошлом.");
        if (StartTime < TimeSpan.FromHours(8) || StartTime > TimeSpan.FromHours(22))
            ModelState.AddModelError(nameof(StartTime), "Бронирование с 08:00 до 22:00.");
        if (Guests < 1 || Guests > 100)
            ModelState.AddModelError(nameof(Guests), "Гостей: от 1 до 100.");

        if (!ModelState.IsValid) return Page();

        var user = await _users.GetUserAsync(User);
        if (user == null) return Challenge();

        var validIds = AllServices.Select(s => s.Id).ToHashSet();

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

        TempData["Msg"] = "Бронирование успешно создано! Статус можно отслеживать в личном кабинете.";
        return RedirectToPage("/Account/Cabinet");
    }
}