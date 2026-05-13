using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servicio.Atraccion.Business.DTOs.Booking;
using Servicio.Atraccion.Business.Interfaces;
using System.Security.Claims;

namespace Servicio.Atraccion.API.Controllers.V1;

/// <summary>
/// Contrato REST público para la gestión de Reservas y Cancelaciones.
/// Este controlador maneja la parte transaccional del sistema.
/// </summary>
[ApiController]
[Route("api/v1/booking")]
[Authorize] // Requiere que el cliente/usuario esté autenticado
[Produces("application/json")]
public class AtraccionesBookingController : ControllerBase
{
    private readonly IBookingIntegrationService _bookingService;

    public AtraccionesBookingController(IBookingIntegrationService bookingService)
    {
        _bookingService = bookingService;
    }

    /// <summary>
    /// Crea una nueva reserva bloqueando el inventario de cupos.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<AtraccionBookingResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<AtraccionBookingResponseDto>>> CrearReserva([FromBody] AtraccionBookingRequestDto request)
    {
        // Normalizar: convierte passengers→tickets y contactName→billing si es necesario
        request.Normalize();

        var userId = GetUserId();
        var result = await _bookingService.CrearReservaAsync(request, userId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Cancela una reserva y libera los cupos en el inventario.
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<bool>>> CancelarReserva(Guid id)
    {
        var userId = GetUserId();
        var result = await _bookingService.CancelarReservaAsync(id, userId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    /// <summary>
    /// Lista el historial de reservas del usuario autenticado.
    /// </summary>
    [HttpGet("mis-reservas")]
    [ProducesResponseType(typeof(ApiResponse<List<AtraccionBookingResponseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<AtraccionBookingResponseDto>>>> ListarMisReservas()
    {
        var userId = GetUserId();
        var result = await _bookingService.ListarMisReservasAsync(userId);
        return Ok(result);
    }

    /// <summary>
    /// Genera y descarga la factura en formato PDF.
    /// </summary>
    [HttpGet("{id:guid}/invoice/pdf")]
    [Produces("application/pdf")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DescargarFacturaPdf(Guid id)
    {
        // En un caso real, validaríamos que la reserva pertenezca al usuario (GetUserId())
        // Por ahora, permitimos la descarga por el ID de la reserva.
        var pdfBytes = await _bookingService.GenerarPdfFacturaAsync(id);
        
        if (pdfBytes == null)
            return NotFound(new { message = "Factura no encontrada para esta reserva." });

        return File(pdfBytes, "application/pdf", $"Factura-{id}.pdf");
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            throw new UnauthorizedAccessException("Usuario no identificado en el token.");

        return Guid.Parse(userIdClaim);
    }
}
