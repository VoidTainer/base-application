using BaseApplication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BaseApplication.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider sp)
    {
        var db = sp.GetRequiredService<AppDbContext>();
        await db.Database.EnsureCreatedAsync();

        var roleMgr = sp.GetRequiredService<RoleManager<IdentityRole>>();
        foreach (var r in new[] { "Admin", "Manager", "User" })
            if (!await roleMgr.RoleExistsAsync(r))
                await roleMgr.CreateAsync(new IdentityRole(r));

        var userMgr = sp.GetRequiredService<UserManager<ApplicationUser>>();

        async Task<ApplicationUser> EnsureUser(string email, string pass, string role, string name)
        {
            var u = await userMgr.FindByEmailAsync(email);
            if (u == null)
            {
                u = new ApplicationUser { Email = email, UserName = email, FullName = name, EmailConfirmed = true };
                await userMgr.CreateAsync(u, pass);
            }
            if (!await userMgr.IsInRoleAsync(u, role))
                await userMgr.AddToRoleAsync(u, role);
            return u;
        }

        await EnsureUser("admin@base.ru", "Admin123", "Admin", "Администратор");
        await EnsureUser("manager@base.ru", "Manager123", "Manager", "Менеджер");
        await EnsureUser("user@base.ru", "User1234", "User", "Иван Иванов");

        if (!await db.Services.AnyAsync())
        {
            db.Services.AddRange(
                new Service { Name = "Баня", Description = "Русская баня на дровах с парилкой.", Price = 3000 },
                new Service { Name = "Аренда инвентаря", Description = "Мангал, шампуры, спортинвентарь.", Price = 500 },
                new Service { Name = "Питание", Description = "Трёхразовое питание из фермерских продуктов.", Price = 1500 },
                new Service { Name = "Экскурсия по окрестностям", Description = "Пешая экскурсия с гидом (2 ч).", Price = 800 }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.Reviews.AnyAsync())
        {
            db.Reviews.AddRange(
                new Review { UserId = "seed", AuthorName = "Анна", Rating = 5, Text = "Прекрасное место, тишина и природа!", IsApproved = true },
                new Review { UserId = "seed", AuthorName = "Сергей", Rating = 4, Text = "Хорошая баня, приветливый персонал.", IsApproved = true }
            );
            await db.SaveChangesAsync();
        }
    }
}