using BaseApplication.Data;
using BaseApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BaseApplication.Pages.Account;

[Authorize]
public class CabinetModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _users;
    public CabinetModel(AppDbContext db, UserManager<ApplicationUser> users)
    { _db = db; _users = users; }

    public ApplicationUser? Me { get; set; }
    public List<Booking> Bookings { get; set; } = new();

    public async Task OnGetAsync()
    {
        Me = await _users.GetUserAsync(User);
        Bookings = await _db.Bookings
            .Include(b => b.BookingServices).ThenInclude(bs => bs.Service)
            .Where(b => b.UserId == Me!.Id)
            .OrderByDescending(b => b.Date)
            .ToListAsync();
    }
}