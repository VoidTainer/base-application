using BaseApplication.Data;
using BaseApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BaseApplication.Pages.Admin;

[Authorize(Roles = "Admin")]
public class ServicesModel : PageModel
{
    private readonly AppDbContext _db;
    public ServicesModel(AppDbContext db) => _db = db;

    public List<Service> Items { get; set; } = new();

    [BindProperty] public ServiceEditModel Input { get; set; } = new();

    public class ServiceEditModel
    {
        public int Id { get; set; }
        [Required, MaxLength(100)] public string Name { get; set; } = "";
        [MaxLength(1000)] public string Description { get; set; } = "";
        [Range(0, 1_000_000)] public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public async Task OnGetAsync() => Items = await _db.Services.OrderBy(s => s.Name).ToListAsync();

    public async Task<IActionResult> OnPostSaveAsync()
    {
        if (!ModelState.IsValid)
        {
            await OnGetAsync();
            return Page();
        }

        if (Input.Id == 0)
        {
            _db.Services.Add(new Service
            {
                Name = Input.Name.Trim(),
                Description = Input.Description,
                Price = Input.Price,
                ImageUrl = Input.ImageUrl,
                IsActive = Input.IsActive
            });
        }
        else
        {
            var s = await _db.Services.FindAsync(Input.Id);
            if (s == null) return NotFound();
            s.Name = Input.Name.Trim();
            s.Description = Input.Description;
            s.Price = Input.Price;
            s.ImageUrl = Input.ImageUrl;
            s.IsActive = Input.IsActive;
        }
        await _db.SaveChangesAsync();
        TempData["Msg"] = "Услуга сохранена.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var s = await _db.Services.FindAsync(id);
        if (s == null) return NotFound();
        var used = await _db.BookingServices.AnyAsync(bs => bs.ServiceId == id);
        if (used) { s.IsActive = false; TempData["Msg"] = "Услуга деактивирована (есть бронирования)."; }
        else { _db.Services.Remove(s); TempData["Msg"] = "Услуга удалена."; }
        await _db.SaveChangesAsync();
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostEditAsync(int id)
    {
        var s = await _db.Services.FindAsync(id);
        if (s == null) return NotFound();
        Input = new ServiceEditModel
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description,
            Price = s.Price,
            ImageUrl = s.ImageUrl,
            IsActive = s.IsActive
        };
        await OnGetAsync();
        return Page();
    }
}