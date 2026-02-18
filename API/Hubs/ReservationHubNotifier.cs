using MedicineFinder.Application.DTOs;
using MedicineFinder.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace MedicineFinder.API.Hubs;

public class ReservationHubNotifier : IReservationNotifier
{
    private readonly IHubContext<StockHub> _hubContext;

    public ReservationHubNotifier(IHubContext<StockHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task ReservationUpdatedAsync(ReservationUpdatedDto update)
    {
        return _hubContext.Clients.All.SendAsync("ReservationUpdated", update);
    }
}
