namespace MedicineFinder.Application.DTOs;

public class ReservationUpdatedDto
{
    public Guid ReservationId { get; set; }
    public Guid UserId { get; set; }
    public Guid PharmacyId { get; set; }
    public Guid MedicineId { get; set; }
    public int Quantity { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? RejectionReason { get; set; }
    public DateTime UpdatedAt { get; set; }
}
