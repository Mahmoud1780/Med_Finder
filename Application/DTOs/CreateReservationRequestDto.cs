using MedicineFinder.Domain.Enums;

namespace MedicineFinder.Application.DTOs;

public class CreateReservationRequestDto
{
    public Guid PharmacyId { get; set; }
    public Guid MedicineId { get; set; }
    public int Quantity { get; set; }
}
