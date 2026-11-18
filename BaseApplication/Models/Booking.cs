using BaseApplication.Models;
using System.ComponentModel.DataAnnotations;

namespace BaseApplication.Models;

public enum BookingStatus { Pending, Confirmed, Cancelled }

public class Booking
{
    public int Id { get; set; }
    public string UserId { get; set; } = "";

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public TimeSpan StartTime { get; set; }

    [Range(1, 100)]
    public int Guests { get; set; } = 1;

    public BookingStatus Status { get; set; } = BookingStatus.Pending;
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<BookingService> BookingServices { get; set; } = new();
}

public class BookingService
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public Booking? Booking { get; set; }
    public int ServiceId { get; set; }
    public Service? Service { get; set; }
}