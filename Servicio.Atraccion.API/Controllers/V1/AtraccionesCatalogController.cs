using Microsoft.AspNetCore.Mvc;
using Servicio.Atraccion.Business.DTOs.Booking;
using Servicio.Atraccion.Business.Interfaces;

namespace Servicio.Atraccion.API.Controllers.V1;

/// <summary>
/// Contrato REST público del Servicio de Atracciones para el Catálogo (Búsqueda y Disponibilidad).
/// Este controlador está optimizado para que Booking pueda "espejar" los productos.
/// </summary>
[ApiController]
[Route("api/v1/atracciones")]
[Produces("application/json")]
public class AtraccionesCatalogController : ControllerBase
{
    private readonly IBookingIntegrationService _bookingService;

    public AtraccionesCatalogController(IBookingIntegrationService bookingService)
    {
        _bookingService = bookingService;
    }

    /// <summary>
    /// Lista atracciones disponibles con paginación y filtros básicos.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedApiResponse<AtraccionBookingDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedApiResponse<AtraccionBookingDto>>> ListarAtracciones(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? ubicacion = null,
        [FromQuery] bool? disponible = null,
        [FromQuery] decimal? precioMinimo = null,
        [FromQuery] decimal? precioMaximo = null)
    {
        var request = new AtraccionSearchBookingRequest
        {
            Page = page,
            PageSize = pageSize,
            Search = search,
            Ubicacion = ubicacion,
            Disponible = disponible,
            PrecioMinimo = precioMinimo,
            PrecioMaximo = precioMaximo
        };

        var result = await _bookingService.ListarAtraccionesAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Obtiene el detalle de una atracción para sincronización de datos.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<AtraccionBookingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<AtraccionBookingDto>>> ObtenerAtraccion(Guid id)
    {
        var result = await _bookingService.ObtenerAtraccionAsync(id);

        if (!result.Success)
            return NotFound(result);

        return Ok(result);
    }

    /// <summary>
    /// Consulta la disponibilidad agregada por día.
    /// </summary>
    [HttpGet("{id:guid}/disponibilidad")]
    [ProducesResponseType(typeof(ApiResponse<List<DisponibilidadDiariaDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<DisponibilidadDiariaDto>>>> ObtenerDisponibilidad(
        Guid id,
        [FromQuery] DateOnly? fecha = null)
    {
        var result = await _bookingService.ObtenerDisponibilidadAsync(id, fecha);
        return Ok(result);
    }
}
