using MedicineFinder.Domain.Enums;

namespace MedicineFinder.Application.DTOs;

public class MedicineSearchResultDto
{
    public Guid MedicineId { get; set; }
    public string MedicineName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string ActiveIngredient { get; set; } = string.Empty;
    public Guid PharmacyId { get; set; }
    public string PharmacyName { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int Quantity { get; set; }
    public AvailabilityStatus Availability { get; set; }
    public double? DistanceKm { get; set; }
}
