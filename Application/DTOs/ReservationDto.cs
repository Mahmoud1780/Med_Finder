using MedicineFinder.Domain.Enums;

namespace MedicineFinder.Application.DTOs;

public class ReservationDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid PharmacyId { get; set; }
    public Guid MedicineId { get; set; }
    public int Quantity { get; set; }
    public ReservationStatus Status { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime CreatedAt { get; set; }
}
