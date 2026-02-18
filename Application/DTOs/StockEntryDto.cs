namespace MedicineFinder.Application.DTOs;

public class StockEntryDto
{
    public Guid PharmacyId { get; set; }
    public string PharmacyName { get; set; } = string.Empty;
    public Guid MedicineId { get; set; }
    public string MedicineName { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
