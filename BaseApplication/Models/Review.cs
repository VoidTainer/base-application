using System.ComponentModel.DataAnnotations;

namespace BaseApplication.Models;

public class Review
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";
    public string AuthorName { get; set; } = "";

    [Range(1, 5)]
    public int Rating { get; set; } = 5;

    [Required, MaxLength(1000)]
    public string Text { get; set; } = "";

    public bool IsApproved { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}