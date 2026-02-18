namespace MedicineFinder.Application.DTOs;

public class MedicineAlternativeDto
{
    public Guid MedicineId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string ActiveIngredient { get; set; } = string.Empty;
    public int TrendingScore { get; set; }
}
