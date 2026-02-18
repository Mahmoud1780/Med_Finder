using MedicineFinder.Domain.Enums;

namespace MedicineFinder.Application.DTOs;

public class MedicineSearchRequestDto
{
    public string Keyword { get; set; } = string.Empty;
    public bool InStockOnly { get; set; }
    public SortBy SortBy { get; set; } = SortBy.HighestStock;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}
