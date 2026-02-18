namespace MedicineFinder.Application.DTOs;

public class SearchResponseDto
{
    public IReadOnlyList<MedicineSearchResultDto> Results { get; set; } = Array.Empty<MedicineSearchResultDto>();
    public IReadOnlyList<MedicineAlternativeDto> Alternatives { get; set; } = Array.Empty<MedicineAlternativeDto>();
    public string? Message { get; set; }
}
