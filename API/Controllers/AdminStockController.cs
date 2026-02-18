using MedicineFinder.Application.DTOs;
using MedicineFinder.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicineFinder.API.Controllers;

[ApiController]
[Route("api/v1/admin/stock")]
[Authorize(Roles = "Admin")]
public class AdminStockController : ControllerBase
{
    private readonly IStockService _stockService;

    public AdminStockController(IStockService stockService)
    {
        _stockService = stockService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StockEntryDto>>> GetStock()
    {
        var stock = await _stockService.GetStockEntriesAsync();
        return Ok(stock);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateStock([FromBody] UpdateStockRequestDto request)
    {
        await _stockService.UpdateStockAsync(request);
        return Ok();
    }
}
