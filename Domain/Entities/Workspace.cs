using System;

namespace Domain.Entities;

public class Workspace
{

    public int Id { get; set; }
    public int RoomId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.MinValue;

    public Room? Room { get; set; }
    public ICollection<Booking> Bookings { get; set; } = [];

}
