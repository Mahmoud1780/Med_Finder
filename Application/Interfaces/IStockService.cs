using MedicineFinder.Application.DTOs;

namespace MedicineFinder.Application.Interfaces;

public interface IStockService
{
    Task UpdateStockAsync(UpdateStockRequestDto request);
    Task<IReadOnlyList<StockEntryDto>> GetStockEntriesAsync();
}
