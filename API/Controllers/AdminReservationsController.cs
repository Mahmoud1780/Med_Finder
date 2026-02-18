using MedicineFinder.Application.DTOs;
using MedicineFinder.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicineFinder.API.Controllers;

[ApiController]
[Route("api/v1/admin/reservations")]
[Authorize(Roles = "Admin")]
public class AdminReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public AdminReservationsController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [HttpGet("pending")]
    public async Task<ActionResult<IReadOnlyList<PendingReservationDto>>> GetPending()
    {
        var reservations = await _reservationService.GetPendingReservationsAsync();
        return Ok(reservations);
    }
}
