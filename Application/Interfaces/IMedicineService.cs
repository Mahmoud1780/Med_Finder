using MedicineFinder.Application.DTOs;

namespace MedicineFinder.Application.Interfaces;

public interface IMedicineService
{
    Task<SearchResponseDto> SearchAsync(MedicineSearchRequestDto request);
    Task<IReadOnlyList<MedicineAlternativeDto>> GetAlternativesAsync(Guid? medicineId, string? keyword = null);
}
