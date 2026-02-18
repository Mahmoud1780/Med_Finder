namespace MedicineFinder.Application.DTOs;

public class PendingReservationDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public Guid MedicineId { get; set; }
    public string MedicineName { get; set; } = string.Empty;
    public Guid PharmacyId { get; set; }
    public string PharmacyName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
}
