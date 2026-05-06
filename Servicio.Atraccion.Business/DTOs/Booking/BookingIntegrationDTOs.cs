namespace Servicio.Atraccion.Business.DTOs.Booking;

// ══════════════════════════════════════════════════════════════
// WRAPPER ESTÁNDAR DE RESPUESTA (Compatible con todos los servicios)
// ══════════════════════════════════════════════════════════════

/// <summary>
/// Envoltorio genérico de respuesta REST. Todos los endpoints del contrato
/// de integración con Booking deben retornar este tipo.
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = [];

    public static ApiResponse<T> Ok(T data, string? message = null) =>
        new() { Success = true, Data = data, Message = message };

    public static ApiResponse<T> Fail(string message, List<string>? errors = null) =>
        new() { Success = false, Message = message, Errors = errors ?? [] };
}

/// <summary>
/// Respuesta paginada con metadatos de navegación.
/// </summary>
public class PagedApiResponse<T>
{
    public bool Success { get; set; }
    public List<T> Data { get; set; } = [];
    public PaginationMeta Meta { get; set; } = new();
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = [];

    public static PagedApiResponse<T> Ok(List<T> data, int totalItems, int page, int pageSize) =>
        new()
        {
            Success = true,
            Data = data,
            Meta = new PaginationMeta
            {
                TotalItems = totalItems,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
                CurrentPage = page,
                PageSize = pageSize
            }
        };
}

public class PaginationMeta
{
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
}

// ══════════════════════════════════════════════════════════════
// DTO: ATRACCIÓN PARA INTEGRACIÓN CON BOOKING
// ══════════════════════════════════════════════════════════════

/// <summary>
/// DTO de atracción para el contrato de integración con el sistema de Booking.
/// Sigue el estándar { id, nombre, descripcion, precio, moneda, ubicacion, imagenUrl, disponible }
/// </summary>
public class AtraccionBookingDto
{
    // ── Campos básicos requeridos por Booking.productos ──────────

    /// <summary>
    /// ID de la atracción. Se almacena en productos.id_externo en el sistema de Booking.
    /// </summary>
    public Guid Id { get; set; }

    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }

    /// <summary>
    /// Precio mínimo disponible.
    /// Se almacena en productos.precio en Booking para filtros y ordenamiento.
    /// </summary>
    public decimal Precio { get; set; }

    /// <summary>Código de moneda ISO 4217 (Ej: "USD").</summary>
    public string Moneda { get; set; } = "USD";

    public string Ubicacion { get; set; } = string.Empty;

    /// <summary>
    /// URL de la imagen principal (is_main = true en attraction_media).
    /// Se almacena en productos.imagen_url en Booking.
    /// </summary>
    public string? ImagenUrl { get; set; }

    /// <summary>
    /// True si la atracción está publicada y tiene al menos un slot futuro disponible.
    /// Se almacena en productos.disponible en Booking.
    /// </summary>
    public bool Disponible { get; set; }
}

// ══════════════════════════════════════════════════════════════
// DTO: DISPONIBILIDAD (Agrupada por día — compatible con Booking)
// ══════════════════════════════════════════════════════════════

/// <summary>
/// Disponibilidad diaria de una atracción.
/// Agrupa todos los slots de horarios de un día en un solo cupo total.
/// Compatible con la tabla disponibilidad_productos del sistema de Booking.
/// </summary>
public class DisponibilidadDiariaDto
{
    /// <summary>Fecha en formato yyyy-MM-dd</summary>
    public string Fecha { get; set; } = string.Empty;

    /// <summary>
    /// Suma de capacity_available de todos los slots activos de ese día.
    /// Se almacena en disponibilidad_productos.cupos_disponibles en Booking.
    /// </summary>
    public int CuposDisponibles { get; set; }

    /// <summary>Horarios disponibles en ese día con su detalle individual</summary>
    public List<HorarioDto> Horarios { get; set; } = [];
}

/// <summary>Slot individual de un horario (para uso en el frontend de atracciones)</summary>
public class HorarioDto
{
    public Guid SlotId { get; set; }
    public string HoraInicio { get; set; } = string.Empty; // Formato HH:mm
    public string? HoraFin { get; set; }
    public int CuposDisponibles { get; set; }
    public int CuposTotales { get; set; }
}

// ══════════════════════════════════════════════════════════════
// REQUEST: FILTROS DE BÚSQUEDA PARA INTEGRACIÓN
// ══════════════════════════════════════════════════════════════

/// <summary>
/// Parámetros de búsqueda del contrato de integración con Booking.
/// </summary>
public class AtraccionSearchBookingRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? Search { get; set; }
    public string? Ubicacion { get; set; }
    public bool? Disponible { get; set; }

    /// <summary>Filtrar por fecha de disponibilidad (yyyy-MM-dd)</summary>
    public DateOnly? Fecha { get; set; }

    public decimal? PrecioMinimo { get; set; }
    public decimal? PrecioMaximo { get; set; }
}
