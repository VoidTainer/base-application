using BaseApplication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BaseApplication.Pages.Admin;

[Authorize(Roles = "Admin")]
public class UsersModel : PageModel
{
    private readonly UserManager<ApplicationUser> _users;
    public UsersModel(UserManager<ApplicationUser> users) => _users = users;

    public List<UserRow> Items { get; set; } = new();

    public class UserRow
    {
        public string Id { get; set; } = "";
        public string Email { get; set; } = "";
        public string? FullName { get; set; }
        public string Role { get; set; } = "";
    }

    public async Task OnGetAsync()
    {
        foreach (var u in await _users.Users.ToListAsync())
        {
            var roles = await _users.GetRolesAsync(u);
            Items.Add(new UserRow
            {
                Id = u.Id,
                Email = u.Email ?? "",
                FullName = u.FullName,
                Role = roles.FirstOrDefault() ?? "—"
            });
        }
    }

    public async Task<IActionResult> OnPostSetRoleAsync(string id, string role)
    {
        if (!new[] { "Admin", "Manager", "User" }.Contains(role)) return BadRequest();
        var u = await _users.FindByIdAsync(id);
        if (u == null) return NotFound();

        var current = await _users.GetRolesAsync(u);
        await _users.RemoveFromRolesAsync(u, current);
        await _users.AddToRoleAsync(u, role);

        TempData["Msg"] = $"Роль пользователя {u.Email} изменена на {role}.";
        return RedirectToPage();
    }
}