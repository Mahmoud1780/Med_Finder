using MedicineFinder.Application.DTOs;
using MedicineFinder.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MedicineFinder.API.Controllers;

[ApiController]
[Route("api/v1/medicines")]
public class MedicinesController : ControllerBase
{
    private readonly IMedicineService _medicineService;

    public MedicinesController(IMedicineService medicineService)
    {
        _medicineService = medicineService;
    }

    [HttpGet("search")]
    public async Task<ActionResult<SearchResponseDto>> Search([FromQuery] MedicineSearchRequestDto request)
    {
        var response = await _medicineService.SearchAsync(request);
        return Ok(response);
    }

    [HttpGet("{id:guid}/alternatives")]
    public async Task<ActionResult<IReadOnlyList<MedicineAlternativeDto>>> Alternatives(Guid id)
    {
        var alternatives = await _medicineService.GetAlternativesAsync(id);
        return Ok(alternatives);
    }
}
