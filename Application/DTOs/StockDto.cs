namespace MedicineFinder.Application.DTOs;

public class StockDto
{
    public Guid MedicineId { get; set; }
    public string MedicineName { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
