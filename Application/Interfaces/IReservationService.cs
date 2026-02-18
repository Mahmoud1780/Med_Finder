using MedicineFinder.Application.DTOs;

namespace MedicineFinder.Application.Interfaces;

public interface IReservationService
{
    Task<ReservationDto> CreateReservationAsync(Guid userId, CreateReservationRequestDto request);
    Task<ReservationDto> ApproveReservationAsync(Guid reservationId);
    Task<ReservationDto> RejectReservationAsync(Guid reservationId, RejectReservationRequestDto request);
    Task<IReadOnlyList<UserReservationItemDto>> GetUserReservationsAsync(Guid userId);
    Task<IReadOnlyList<PendingReservationDto>> GetPendingReservationsAsync();
}
