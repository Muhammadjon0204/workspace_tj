using System;

namespace Domain.Entities;

public class Room
{

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public decimal PricePerHour { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.MinValue;

}
