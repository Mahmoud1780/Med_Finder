using MedicineFinder.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace MedicineFinder.API.Hubs;

public class StockHubNotifier : IStockNotifier
{
    private readonly IHubContext<StockHub> _hubContext;

    public StockHubNotifier(IHubContext<StockHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task StockUpdatedAsync(Guid pharmacyId, Guid medicineId, int quantity)
    {
        return _hubContext.Clients.All.SendAsync("StockUpdated", pharmacyId, medicineId, quantity);
    }
}
