using MedicineFinder.Domain.Enums;

namespace MedicineFinder.Domain.Entities;

public class Reservation
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid PharmacyId { get; set; }
    public Guid MedicineId { get; set; }
    public int Quantity { get; set; }
    public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    public string? RejectionReason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser? User { get; set; }
    public Pharmacy? Pharmacy { get; set; }
    public Medicine? Medicine { get; set; }
}
