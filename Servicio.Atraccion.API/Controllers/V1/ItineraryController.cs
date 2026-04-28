using Microsoft.AspNetCore.Mvc;
using Servicio.Atraccion.Business.DTOs.Attraction;
using Servicio.Atraccion.Business.Interfaces;

namespace Servicio.Atraccion.API.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
public class ItineraryController : ControllerBase
{
    private readonly IAttractionService _attractionService;

    public ItineraryController(IAttractionService attractionService)
    {
        _attractionService = attractionService;
    }

    [HttpGet("attraction/{attractionId}")]
    public async Task<ActionResult<IEnumerable<ItineraryResponse>>> GetByAttraction(Guid attractionId)
    {
        var result = await _attractionService.GetItinerariesAsync(attractionId);
        return Ok(result);
    }

    [HttpPost("attraction/{attractionId}")]
    public async Task<ActionResult<Guid>> Create(Guid attractionId, CreateItineraryRequest request)
    {
        var id = await _attractionService.CreateItineraryAsync(attractionId, request);
        return Ok(id);
    }

    [HttpPost("{itineraryId}/stop")]
    public async Task<ActionResult<Guid>> AddStop(Guid itineraryId, CreateTourStopRequest request)
    {
        var id = await _attractionService.AddStopAsync(itineraryId, request);
        return Ok(id);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _attractionService.DeleteItineraryAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpDelete("stop/{id}")]
    public async Task<IActionResult> DeleteStop(Guid id)
    {
        var result = await _attractionService.DeleteStopAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
}
