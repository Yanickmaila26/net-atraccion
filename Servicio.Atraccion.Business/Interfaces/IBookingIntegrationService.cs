using Servicio.Atraccion.Business.DTOs.Booking;

namespace Servicio.Atraccion.Business.Interfaces;

/// <summary>
/// Servicio de integración con el sistema central de Booking.
/// Expone los endpoints del contrato REST público del servicio de Atracciones.
/// </summary>
public interface IBookingIntegrationService
{
    /// <summary>
    /// Lista atracciones paginadas, filtradas y listas para el espejo en productos de Booking.
    /// Incluye precioDesde (para productos.precio) y lista de modalidades+tarifas (para productos.metadata).
    /// </summary>
    Task<PagedApiResponse<AtraccionBookingDto>> ListarAtraccionesAsync(AtraccionSearchBookingRequest request);

    /// <summary>
    /// Obtiene el detalle completo de una atracción por su ID.
    /// Se usa cuando Booking necesita refrescar los datos del espejo.
    /// </summary>
    Task<ApiResponse<AtraccionBookingDto>> ObtenerAtraccionAsync(Guid id);

    /// <summary>
    /// Consulta la disponibilidad de una atracción agrupada por día.
    /// Compatible con la estructura disponibilidad_productos de Booking.
    /// Si no se especifica fecha, devuelve los próximos 30 días.
    /// </summary>
    Task<ApiResponse<List<DisponibilidadDiariaDto>>> ObtenerDisponibilidadAsync(Guid attractionId, DateOnly? fecha = null);
}
