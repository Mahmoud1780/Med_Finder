using System.Security.Claims;
using MedicineFinder.Application.DTOs;
using MedicineFinder.Application.Exceptions;
using MedicineFinder.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicineFinder.API.Controllers;

[ApiController]
[Route("api/v1/reservations")]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationsController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ReservationDto>> Create([FromBody] CreateReservationRequestDto request)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdValue, out var userId))
        {
            throw new UnauthorizedException("Invalid user context");
        }

        var reservation = await _reservationService.CreateReservationAsync(userId, request);
        return Ok(reservation);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<IReadOnlyList<UserReservationItemDto>>> GetMine()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdValue, out var userId))
        {
            throw new UnauthorizedException("Invalid user context");
        }

        var reservations = await _reservationService.GetUserReservationsAsync(userId);
        return Ok(reservations);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{id:guid}/approve")]
    public async Task<ActionResult<ReservationDto>> Approve(Guid id)
    {
        var reservation = await _reservationService.ApproveReservationAsync(id);
        return Ok(reservation);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{id:guid}/reject")]
    public async Task<ActionResult<ReservationDto>> Reject(Guid id, [FromBody] RejectReservationRequestDto request)
    {
        var reservation = await _reservationService.RejectReservationAsync(id, request);
        return Ok(reservation);
    }
}
