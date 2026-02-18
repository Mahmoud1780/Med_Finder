using MedicineFinder.Application.DTOs;
using MedicineFinder.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MedicineFinder.API.Controllers;

[ApiController]
[Route("api/v1/pharmacies")]
public class PharmaciesController : ControllerBase
{
    private readonly IPharmacyService _pharmacyService;

    public PharmaciesController(IPharmacyService pharmacyService)
    {
        _pharmacyService = pharmacyService;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PharmacyDto>> GetById(Guid id)
    {
        var pharmacy = await _pharmacyService.GetByIdAsync(id);
        return Ok(pharmacy);
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PharmacyDto>>> GetAll()
    {     var pharmacies = await _pharmacyService.GetAllAsync();
        return Ok(pharmacies);
    }
}
