using BaseApplication.Data;
using BaseApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BaseApplication.Pages.Admin;

[Authorize(Roles = "Admin,Manager")]
public class IndexModel : PageModel
{
    private readonly AppDbContext _db;
    public IndexModel(AppDbContext db) => _db = db;

    public int TotalBookings { get; set; }
    public int PendingBookings { get; set; }
    public int TotalReviews { get; set; }
    public int PendingReviews { get; set; }
    public int TotalServices { get; set; }
    public int UnreadMessages { get; set; }
    public List<Booking> RecentBookings { get; set; } = new();

    public async Task OnGetAsync()
    {
        TotalBookings = await _db.Bookings.CountAsync();
        PendingBookings = await _db.Bookings.CountAsync(b => b.Status == BookingStatus.Pending);
        TotalReviews = await _db.Reviews.CountAsync();
        PendingReviews = await _db.Reviews.CountAsync(r => !r.IsApproved);
        TotalServices = await _db.Services.CountAsync();
        UnreadMessages = await _db.ContactMessages.CountAsync(m => !m.IsRead);

        RecentBookings = await _db.Bookings
            .Include(b => b.BookingServices).ThenInclude(bs => bs.Service)
            .OrderByDescending(b => b.CreatedAt)
            .Take(5).ToListAsync();
    }
}