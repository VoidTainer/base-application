using BaseApplication.Data;
using BaseApplication.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BaseApplication.Pages;

public class ServicesModel : PageModel
{
    private readonly AppDbContext _db;
    public ServicesModel(AppDbContext db) => _db = db;

    public List<Service> Items { get; set; } = new();

    public async Task OnGetAsync()
    {
        Items = await _db.Services
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }
}