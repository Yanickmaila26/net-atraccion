using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servicio.Atraccion.Business.DTOs.Attraction;
using Servicio.Atraccion.Business.Interfaces;
using Servicio.Atraccion.DataAccess.Common;
using System.Security.Claims;

namespace Servicio.Atraccion.API.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
public class AttractionController : ControllerBase
{
    private readonly IAttractionService _attractionService;

    public AttractionController(IAttractionService attractionService)
    {
        _attractionService = attractionService;
    }

    /// <summary>
    /// Busca y pagina atracciones según los filtros proporcionados.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResult<AttractionSummaryResponse>>> Search([FromQuery] AttractionSearchRequest request)
    {
        var result = await _attractionService.SearchAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Búsqueda para el panel administrativo. 
    /// Filtra automáticamente por Partner si el usuario no es Admin.
    /// </summary>
    [HttpGet("management")]
    [Authorize(Roles = "Admin,Partner")]
    public async Task<ActionResult<PagedResult<AttractionSummaryResponse>>> SearchManagement([FromQuery] AttractionSearchRequest request)
    {
        var currentUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        bool isAdmin = User.IsInRole("Admin");

        var result = await _attractionService.SearchManagementAsync(request, currentUserId, isAdmin);
        return Ok(result);
    }

    /// <summary>
    /// Crea una nueva atracción. Acceso restringido a Admin y Partner.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Partner")]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateAttractionRequest request)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        bool isAdmin = User.IsInRole("Admin");
        var id = await _attractionService.CreateAsync(request, userId, isAdmin);
        return CreatedAtAction(nameof(Search), new { id }, id);
    }

    /// <summary>
    /// Crea una atracción completa con todos sus detalles, productos, precios y disponibilidad en una sola transacción.
    /// </summary>
    [HttpPost("complete")]
    [Authorize(Roles = "Admin,Partner")]
    public async Task<ActionResult<Guid>> CreateComplete([FromBody] CreateCompleteAttractionRequest request)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        bool isAdmin = User.IsInRole("Admin");
        var id = await _attractionService.CreateCompleteAsync(request, userId, isAdmin);
        return Ok(new { id, message = "Atracción completa creada con éxito." });
    }

    /// <summary>
    /// Obtiene las atracciones mejor valoradas.
    /// </summary>
    [HttpGet("top")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<AttractionSummaryResponse>>> GetTopRated([FromQuery] int count = 5)
    {
        var result = await _attractionService.GetTopRatedAsync(count);
        return Ok(result);
    }

    /// <summary>
    /// Obtiene el detalle completo de una atracción por su Slug (URL amigable).
    /// </summary>
    [HttpGet("{slug}")]
    [AllowAnonymous]
    public async Task<ActionResult<AttractionDetailResponse>> GetBySlug(string slug, [FromQuery] short requestedLangId = 1)
    {
        var result = await _attractionService.GetDetailBySlugAsync(slug, requestedLangId);
        return Ok(result);
    }

    /// <summary>
    /// Obtiene el calendario de disponibilidad de espacios para una atracción en un rango de fechas.
    /// </summary>
    [HttpGet("{attractionId:guid}/availability")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<Business.DTOs.Inventory.AvailabilitySlotResponse>>> GetAvailability(
        Guid attractionId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var request = new Business.DTOs.Inventory.AvailabilityRequest
        {
            AttractionId = attractionId,
            StartDate = startDate,
            EndDate = endDate
        };
        var result = await _attractionService.GetAvailabilityAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Edita una atracción existente (Admin y Partner propietario).
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Partner")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateAttractionRequest request)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        bool isAdmin = User.IsInRole("Admin");
        var success = await _attractionService.UpdateAsync(id, request, userId, isAdmin);
        if (!success) return NotFound();
        return Ok(new { message = "Atracción actualizada con éxito." });
    }

    /// <summary>
    /// Elimina una atracción permanentemente (Solo Admin).
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var success = await _attractionService.DeleteAsync(id, userId, isAdmin: true);
        if (!success) return NotFound();
        return Ok(new { message = "Atracción eliminada con éxito." });
    }

    /// <summary>
    /// Publica o despublica una atracción (Admin y Partner propietario).
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Admin,Partner")]
    public async Task<ActionResult> ToggleStatus(Guid id, [FromBody] ToggleAttractionStatusRequest request)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        bool isAdmin = User.IsInRole("Admin");
        var success = await _attractionService.ToggleStatusAsync(id, request.IsPublished, userId, isAdmin);
        if (!success) return NotFound();
        var estado = request.IsPublished ? "publicada" : "despublicada";
        return Ok(new { message = $"Atracción {estado} con éxito." });
    }

    /// <summary>
    /// Activa o desactiva lógicamente una atracción. 
    /// Si se desactiva, se valida que no existan reservas futuras.
    /// </summary>
    [HttpPatch("{id:guid}/active")]
    [Authorize(Roles = "Admin,Partner")]
    public async Task<ActionResult> ToggleActive(Guid id, [FromBody] Servicio.Atraccion.Business.DTOs.Attraction.ToggleAttractionActiveRequest request)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        bool isAdmin = User.IsInRole("Admin");
        
        try
        {
            var success = await _attractionService.ToggleActiveAsync(id, request.IsActive, userId, isAdmin);
            if (!success) return NotFound();
            
            var estado = request.IsActive ? "activada" : "desactivada";
            return Ok(new { message = $"Atracción {estado} con éxito." });
        }
        catch (Business.Exceptions.ValidationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Obtiene el detalle completo de una atracción incluyendo plantillas de horarios para edición.
    /// </summary>
    [HttpGet("{id:guid}/complete")]
    [Authorize(Roles = "Admin,Partner")]
    public async Task<ActionResult<Servicio.Atraccion.Business.DTOs.Attraction.AttractionFullEditionResponse>> GetComplete(Guid id)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        bool isAdmin = User.IsInRole("Admin");

        var result = await _attractionService.GetCompleteByIdAsync(id, userId, isAdmin);
        if (result == null) return NotFound();

        return Ok(result);
    }
}