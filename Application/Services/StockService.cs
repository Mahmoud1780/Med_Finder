using MedicineFinder.Application.DTOs;
using MedicineFinder.Application.Interfaces;
using MedicineFinder.Domain.Entities;

namespace MedicineFinder.Application.Services;

public class StockService : IStockService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStockNotifier _notifier;

    public StockService(IUnitOfWork unitOfWork, IStockNotifier notifier)
    {
        _unitOfWork = unitOfWork;
        _notifier = notifier;
    }

    public async Task UpdateStockAsync(UpdateStockRequestDto request)
    {
        var stockRepo = _unitOfWork.Repository<Stock>();
        var stocks = await stockRepo.FindAsync(s => s.PharmacyId == request.PharmacyId && s.MedicineId == request.MedicineId);
        var stock = stocks.FirstOrDefault();

        if (stock == null)
        {
            stock = new Stock
            {
                Id = Guid.NewGuid(),
                PharmacyId = request.PharmacyId,
                MedicineId = request.MedicineId,
                Quantity = request.Quantity
            };

            await stockRepo.AddAsync(stock);
        }
        else
        {
            stock.Quantity = request.Quantity;
            stockRepo.Update(stock);
        }

        await _unitOfWork.SaveChangesAsync();
        await _notifier.StockUpdatedAsync(request.PharmacyId, request.MedicineId, request.Quantity);
    }

    public async Task<IReadOnlyList<StockEntryDto>> GetStockEntriesAsync()
    {
        var stockRepo = _unitOfWork.Repository<Stock>();
        var stocks = await stockRepo.GetAllAsync();

        if (stocks.Count == 0)
        {
            return Array.Empty<StockEntryDto>();
        }

        var pharmacyRepo = _unitOfWork.Repository<Pharmacy>();
        var medicineRepo = _unitOfWork.Repository<Medicine>();

        var pharmacyIds = stocks.Select(s => s.PharmacyId).Distinct().ToList();
        var medicineIds = stocks.Select(s => s.MedicineId).Distinct().ToList();

        var pharmacies = await pharmacyRepo.FindAsync(p => pharmacyIds.Contains(p.Id));
        var medicines = await medicineRepo.FindAsync(m => medicineIds.Contains(m.Id));

        var pharmacyMap = pharmacies.ToDictionary(p => p.Id, p => p.Name);
        var medicineMap = medicines.ToDictionary(m => m.Id, m => m.Name);

        return stocks.Select(s => new StockEntryDto
        {
            PharmacyId = s.PharmacyId,
            PharmacyName = pharmacyMap.TryGetValue(s.PharmacyId, out var pname) ? pname : string.Empty,
            MedicineId = s.MedicineId,
            MedicineName = medicineMap.TryGetValue(s.MedicineId, out var mname) ? mname : string.Empty,
            Quantity = s.Quantity
        }).ToList();
    }
}
