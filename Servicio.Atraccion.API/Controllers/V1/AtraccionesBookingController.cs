using Microsoft.AspNetCore.Mvc;
using Servicio.Atraccion.Business.DTOs.Booking;
using Servicio.Atraccion.Business.Interfaces;

namespace Servicio.Atraccion.API.Controllers.V1;

/// <summary>
/// Contrato REST público del Servicio de Atracciones para integración con el sistema central de Booking.
/// Todos los endpoints siguen el estándar { success, data, message, errors }.
/// </summary>
[ApiController]
[Route("api/v1/atracciones")]
[Produces("application/json")]
public class AtraccionesBookingController : ControllerBase
{
    private readonly IBookingIntegrationService _bookingService;

    public AtraccionesBookingController(IBookingIntegrationService bookingService)
    {
        _bookingService = bookingService;
    }

    /// <summary>
    /// Lista atracciones disponibles con paginación y filtros.
    /// Retorna precio para el campo precio de productos en Booking.
    /// </summary>
    /// <param name="page">Número de página (default: 1)</param>
    /// <param name="pageSize">Tamaño de página (default: 20)</param>
    /// <param name="search">Búsqueda por nombre o descripción</param>
    /// <param name="ubicacion">Filtrar por nombre de ubicación</param>
    /// <param name="disponible">Solo atracciones con cupos disponibles futuros</param>
    /// <param name="precioMinimo">Filtrar por precio mínimo (sobre precio)</param>
    /// <param name="precioMaximo">Filtrar por precio máximo (sobre precio)</param>
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
    /// Obtiene el detalle completo de una atracción por su ID.
    /// Usado por Booking para crear o refrescar el espejo en la tabla productos.
    /// El campo id se almacena en productos.id_externo.
    /// </summary>
    /// <param name="id">UUID de la atracción</param>
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
    /// Consulta la disponibilidad de una atracción agrupada por día.
    /// Compatible con la tabla disponibilidad_productos del sistema de Booking:
    /// - CuposDisponibles = SUM(capacity_available) de todos los slots del día.
    /// - Si no se especifica fecha, devuelve los próximos 30 días.
    /// </summary>
    /// <param name="id">UUID de la atracción</param>
    /// <param name="fecha">Fecha específica en formato yyyy-MM-dd (opcional)</param>
    [HttpGet("{id:guid}/disponibilidad")]
    [ProducesResponseType(typeof(ApiResponse<List<DisponibilidadDiariaDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<List<DisponibilidadDiariaDto>>>> ObtenerDisponibilidad(
        Guid id,
        [FromQuery] DateOnly? fecha = null)
    {
        var result = await _bookingService.ObtenerDisponibilidadAsync(id, fecha);
        return Ok(result);
    }
}
