using System.ComponentModel.DataAnnotations;

namespace BaseApplication.Models;

public class ContactMessage
{
    public int Id { get; set; }
    public string? UserId { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = "";

    [Required, EmailAddress, MaxLength(150)]
    public string Email { get; set; } = "";

    [Required, MaxLength(2000)]
    public string Message { get; set; } = "";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; }
}