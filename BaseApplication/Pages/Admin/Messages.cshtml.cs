using BaseApplication.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BaseApplication.Pages.Admin;

[Authorize(Roles = "Admin,Manager")]
public class MessagesModel : PageModel
{
    private readonly AppDbContext _db;
    public MessagesModel(AppDbContext db) => _db = db;

    public List<BaseApplication.Models.ContactMessage> Items { get; set; } = new();

    public async Task OnGetAsync() =>
        Items = await _db.ContactMessages.OrderByDescending(m => m.CreatedAt).ToListAsync();

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> OnPostMarkReadAsync(int id)
    {
        var m = await _db.ContactMessages.FindAsync(id);
        if (m == null) return NotFound();
        m.IsRead = true;
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var m = await _db.ContactMessages.FindAsync(id);
        if (m == null) return NotFound();
        _db.ContactMessages.Remove(m);
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }
}