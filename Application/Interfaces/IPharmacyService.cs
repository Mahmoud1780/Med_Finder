using MedicineFinder.Application.DTOs;

namespace MedicineFinder.Application.Interfaces;

public interface IPharmacyService
{
    Task<IEnumerable<PharmacyDto>> GetAllAsync();
    Task<PharmacyDto> GetByIdAsync(Guid pharmacyId);
}
