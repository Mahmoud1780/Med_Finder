using AutoMapper;
using MedicineFinder.Application.DTOs;
using MedicineFinder.Application.Exceptions;
using MedicineFinder.Application.Interfaces;
using MedicineFinder.Domain.Entities;

namespace MedicineFinder.Application.Services;

public class PharmacyService : IPharmacyService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PharmacyService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }


    public async Task<IEnumerable<PharmacyDto>> GetAllAsync()
    {
        var pharmacyRepo = _unitOfWork.Repository<Pharmacy>();
        var pharmacies = await pharmacyRepo.GetAllAsync();
        if (pharmacies == null || pharmacies.Count == 0)
        {
            return Array.Empty<PharmacyDto>();
        }

        var stockRepo = _unitOfWork.Repository<Stock>();
        var stocks = await stockRepo.GetAllAsync();

        var medicineRepo = _unitOfWork.Repository<Medicine>();
        var medicineIds = stocks.Select(s => s.MedicineId).Distinct().ToList();
        var medicines = medicineIds.Count == 0
            ? new List<Medicine>()
            : await medicineRepo.FindAsync(m => medicineIds.Contains(m.Id));
        var medicineMap = medicines.ToDictionary(m => m.Id, m => m.Name);

        foreach (var stock in stocks)
        {
            if (medicineMap.TryGetValue(stock.MedicineId, out var name))
            {
                stock.Medicine = new Medicine { Id = stock.MedicineId, Name = name };
            }
        }

        var result = new List<PharmacyDto>(pharmacies.Count);
        foreach (var pharmacy in pharmacies)
        {
            var dto = _mapper.Map<PharmacyDto>(pharmacy);
            var pharmacyStocks = stocks.Where(s => s.PharmacyId == pharmacy.Id)
                .Select(s => new StockDto
                {
                    MedicineId = s.MedicineId,
                    MedicineName = s.Medicine?.Name ?? string.Empty,
                    Quantity = s.Quantity
                })
                .ToList();

            dto.Stocks = pharmacyStocks;
            result.Add(dto);
        }

        return result;
    }


    public async Task<PharmacyDto> GetByIdAsync(Guid pharmacyId)
    {
        var pharmacyRepo = _unitOfWork.Repository<Pharmacy>();
        var pharmacy = await pharmacyRepo.GetByIdAsync(pharmacyId);
        if (pharmacy == null)
        {
            throw new NotFoundException("Pharmacy not found");
        }

        var stockRepo = _unitOfWork.Repository<Stock>();
        var stocks = await stockRepo.FindAsync(s => s.PharmacyId == pharmacyId);

        var medicineRepo = _unitOfWork.Repository<Medicine>();
        var medicineIds = stocks.Select(s => s.MedicineId).Distinct().ToList();
        var medicines = medicineIds.Count == 0
            ? new List<Medicine>()
            : await medicineRepo.FindAsync(m => medicineIds.Contains(m.Id));
        var medicineMap = medicines.ToDictionary(m => m.Id, m => m.Name);

        foreach (var stock in stocks)
        {
            if (medicineMap.TryGetValue(stock.MedicineId, out var name))
            {
                stock.Medicine = new Medicine { Id = stock.MedicineId, Name = name };
            }
        }

        var dto = _mapper.Map<PharmacyDto>(pharmacy);
        dto.Stocks = stocks.Select(s => new StockDto
        {
            MedicineId = s.MedicineId,
            MedicineName = s.Medicine?.Name ?? string.Empty,
            Quantity = s.Quantity
        }).ToList();

        return dto;
    }
}
