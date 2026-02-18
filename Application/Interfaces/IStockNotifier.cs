namespace MedicineFinder.Application.Interfaces;

public interface IStockNotifier
{
    Task StockUpdatedAsync(Guid pharmacyId, Guid medicineId, int quantity);
}
