using MedicineFinder.Application.DTOs;

namespace MedicineFinder.Application.Interfaces;

public interface IPharmacyService
{
    Task<PharmacyDto> GetByIdAsync(Guid pharmacyId);
}
