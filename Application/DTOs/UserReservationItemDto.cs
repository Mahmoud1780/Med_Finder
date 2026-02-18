namespace MedicineFinder.Application.DTOs;

public class UserReservationItemDto
{
    public Guid Id { get; set; }
    public Guid MedicineId { get; set; }
    public string MedicineName { get; set; } = string.Empty;
    public Guid PharmacyId { get; set; }
    public string PharmacyName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? RejectionReason { get; set; }
    public DateTime CreatedAt { get; set; }
}
