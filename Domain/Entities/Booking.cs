using System;

namespace Domain.Entities;

public class Booking
{

    public int Id { get; set; }
    public int CompanyId { get; set; }
    public int WorkspaceId { get; set; }
    public DateTime BookingDate { get; set; } = DateTime.MinValue;
    public TimeSpan StartTime { get; set; } = TimeSpan.MinValue;
    public TimeSpan EndTime { get; set; } = TimeSpan.MinValue;
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.MinValue;

}
