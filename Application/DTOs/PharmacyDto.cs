namespace MedicineFinder.Application.DTOs;

public class PharmacyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public IReadOnlyList<StockDto> Stocks { get; set; } = Array.Empty<StockDto>();
}
