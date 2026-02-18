namespace MedicineFinder.Application.DTOs;

public class UpdateStockRequestDto
{
    public Guid PharmacyId { get; set; }
    public Guid MedicineId { get; set; }
    public int Quantity { get; set; }
}
