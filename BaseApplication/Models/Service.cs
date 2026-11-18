using System.ComponentModel.DataAnnotations;

namespace BaseApplication.Models;

public class Service
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = "";

    [MaxLength(1000)]
    public string Description { get; set; } = "";

    [Range(0, 1000000)]
    public decimal Price { get; set; }

    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
}