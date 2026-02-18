using MedicineFinder.Application.DTOs;

namespace MedicineFinder.Application.Interfaces;

public interface IReservationNotifier
{
    Task ReservationUpdatedAsync(ReservationUpdatedDto update);
}
