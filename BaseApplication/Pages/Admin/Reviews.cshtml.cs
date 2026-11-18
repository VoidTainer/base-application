using BaseApplication.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BaseApplication.Pages.Admin;

[Authorize(Roles = "Admin,Manager")]
public class ReviewsModel : PageModel
{
    private readonly AppDbContext _db;
    public ReviewsModel(AppDbContext db) => _db = db;

    public List<BaseApplication.Models.Review> Items { get; set; } = new();

    public async Task OnGetAsync() =>
        Items = await _db.Reviews.OrderBy(r => r.IsApproved).ThenByDescending(r => r.CreatedAt).ToListAsync();

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> OnPostApproveAsync(int id)
    {
        var r = await _db.Reviews.FindAsync(id);
        if (r == null) return NotFound();
        r.IsApproved = true;
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var r = await _db.Reviews.FindAsync(id);
        if (r == null) return NotFound();
        _db.Reviews.Remove(r);
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }
}