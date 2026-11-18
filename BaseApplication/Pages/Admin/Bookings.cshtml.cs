using BaseApplication.Data;
using BaseApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BaseApplication.Pages.Admin;

[Authorize(Roles = "Admin,Manager")]
public class BookingsModel : PageModel
{
    private readonly AppDbContext _db;
    public BookingsModel(AppDbContext db) => _db = db;

    public List<Booking> Items { get; set; } = new();

    [BindProperty(SupportsGet = true)] public string? Filter { get; set; }

    public async Task OnGetAsync()
    {
        var q = _db.Bookings
            .Include(b => b.BookingServices).ThenInclude(bs => bs.Service)
            .AsQueryable();

        if (Enum.TryParse<BookingStatus>(Filter, true, out var st))
            q = q.Where(b => b.Status == st);

        Items = await q.OrderByDescending(b => b.CreatedAt).ToListAsync();
    }

    public async Task<IActionResult> OnPostConfirmAsync(int id)
    {
        var b = await _db.Bookings.FindAsync(id);
        if (b == null) return NotFound();
        b.Status = BookingStatus.Confirmed;
        await _db.SaveChangesAsync();
        TempData["Msg"] = $"Бронирование #{id} подтверждено.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCancelAsync(int id)
    {
        var b = await _db.Bookings.FindAsync(id);
        if (b == null) return NotFound();
        b.Status = BookingStatus.Cancelled;
        await _db.SaveChangesAsync();
        TempData["Msg"] = $"Бронирование #{id} отменено.";
        return RedirectToPage();
    }
}