using MedicineFinder.Application.DTOs;
using MedicineFinder.Application.Interfaces;
using MedicineFinder.Domain.Entities;
using MedicineFinder.Domain.Enums;

namespace MedicineFinder.Application.Services;

public class MedicineService : IMedicineService
{
    private readonly IUnitOfWork _unitOfWork;

    public MedicineService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<SearchResponseDto> SearchAsync(MedicineSearchRequestDto request)
    {
        var medicineRepo = _unitOfWork.Repository<Medicine>();
        var stockRepo = _unitOfWork.Repository<Stock>();
        var pharmacyRepo = _unitOfWork.Repository<Pharmacy>();

        var medicines = await medicineRepo.FindAsync(m => m.Name.ToLower().Contains(request.Keyword.ToLower()));
        var response = new SearchResponseDto();

        if (medicines.Count == 0)
        {
            response.Alternatives = await GetAlternativesAsync(null, request.Keyword);
            response.Message = "No matching medicine found.";
            return response;
        }

        var medicineIds = medicines.Select(m => m.Id).ToHashSet();
        var stocks = await stockRepo.FindAsync(s => medicineIds.Contains(s.MedicineId));

        if (request.InStockOnly)
        {
            stocks = stocks.Where(s => s.Quantity > 0).ToList();
        }

        var pharmacyIds = stocks.Select(s => s.PharmacyId).Distinct().ToList();
        var pharmacies = pharmacyIds.Count == 0
            ? new List<Pharmacy>()
            : await pharmacyRepo.FindAsync(p => pharmacyIds.Contains(p.Id));

        var pharmacyMap = pharmacies.ToDictionary(p => p.Id, p => p);
        var medicineMap = medicines.ToDictionary(m => m.Id, m => m);

        var results = new List<MedicineSearchResultDto>();
        foreach (var stock in stocks)
        {
            if (!medicineMap.TryGetValue(stock.MedicineId, out var medicine))
            {
                continue;
            }

            if (!pharmacyMap.TryGetValue(stock.PharmacyId, out var pharmacy))
            {
                continue;
            }

            var result = new MedicineSearchResultDto
            {
                MedicineId = medicine.Id,
                MedicineName = medicine.Name,
                Category = medicine.Category,
                ActiveIngredient = medicine.ActiveIngredient,
                PharmacyId = pharmacy.Id,
                PharmacyName = pharmacy.Name,
                Latitude = pharmacy.Latitude,
                Longitude = pharmacy.Longitude,
                Quantity = stock.Quantity,
                Availability = stock.Quantity > 0 ? AvailabilityStatus.InStock : AvailabilityStatus.OutOfStock
            };

            if (request.SortBy == SortBy.Nearest && request.Latitude.HasValue && request.Longitude.HasValue)
            {
                result.DistanceKm = CalculateDistanceKm(request.Latitude.Value, request.Longitude.Value, pharmacy.Latitude, pharmacy.Longitude);
            }

            results.Add(result);
        }

        if (request.SortBy == SortBy.Nearest)
        {
            results = results.OrderBy(r => r.DistanceKm ?? double.MaxValue).ToList();
        }
        else
        {
            results = results.OrderByDescending(r => r.Quantity).ToList();
        }

        response.Results = results;

        var allOutOfStock = results.Count > 0 && results.All(r => r.Quantity == 0);
        if (results.Count == 0 || allOutOfStock)
        {
            response.Alternatives = await GetAlternativesAsync(null, request.Keyword);
            response.Message = results.Count == 0 ? "No matching medicine found." : "All matching medicines are out of stock.";
        }

        return response;
    }

    public async Task<IReadOnlyList<MedicineAlternativeDto>> GetAlternativesAsync(Guid? medicineId, string? keyword = null)
    {
        var medicineRepo = _unitOfWork.Repository<Medicine>();
        var medicines = await medicineRepo.GetAllAsync();

        Medicine? baseMedicine = null;
        if (medicineId.HasValue)
        {
            baseMedicine = medicines.FirstOrDefault(m => m.Id == medicineId.Value);
        }
        else if (!string.IsNullOrWhiteSpace(keyword))
        {
            var lowered = keyword.ToLower();
            baseMedicine = medicines.FirstOrDefault(m => m.Name.ToLower().Contains(lowered));
        }

        var alternatives = new List<MedicineAlternativeDto>();

        if (baseMedicine != null)
        {
            var related = medicines
                .Where(m => m.Id != baseMedicine.Id &&
                            (m.Category == baseMedicine.Category || m.ActiveIngredient == baseMedicine.ActiveIngredient))
                .OrderByDescending(m => m.TrendingScore)
                .Take(3)
                .Select(MapAlternative)
                .ToList();

            alternatives.AddRange(related);
        }

        if (alternatives.Count < 3)
        {
            var fallback = medicines
                .OrderByDescending(m => m.TrendingScore)
                .Where(m => alternatives.All(a => a.MedicineId != m.Id))
                .Take(3 - alternatives.Count)
                .Select(MapAlternative)
                .ToList();

            alternatives.AddRange(fallback);
        }

        return alternatives;
    }

    private static MedicineAlternativeDto MapAlternative(Medicine medicine)
    {
        return new MedicineAlternativeDto
        {
            MedicineId = medicine.Id,
            Name = medicine.Name,
            Category = medicine.Category,
            ActiveIngredient = medicine.ActiveIngredient,
            TrendingScore = medicine.TrendingScore
        };
    }

    private static double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double radiusKm = 6371.0;
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        var a = Math.Pow(Math.Sin(dLat / 2), 2)
                + Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) * Math.Pow(Math.Sin(dLon / 2), 2);
        var c = 2 * Math.Asin(Math.Sqrt(a));
        return radiusKm * c;
    }

    private static double ToRadians(double angle) => Math.PI * angle / 180.0;
}
