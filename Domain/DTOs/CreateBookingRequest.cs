public class CreateBookingRequest
{
    public int CompanyId { get; set; }

    public int WorkspaceId { get; set; }

    public DateTime BookingDate { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }
}