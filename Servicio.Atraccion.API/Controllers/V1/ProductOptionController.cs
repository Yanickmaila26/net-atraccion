using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servicio.Atraccion.Business.DTOs.Attraction;
using Servicio.Atraccion.Business.DTOs.Inventory;
using Servicio.Atraccion.Business.DTOs.Schedule;
using Servicio.Atraccion.Business.Interfaces;
using Servicio.Atraccion.Business.Services;
using Servicio.Atraccion.DataAccess.Context;
using Microsoft.EntityFrameworkCore;

namespace Servicio.Atraccion.API.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "Admin,Partner")]
public class ProductOptionController : ControllerBase
{
    private readonly IProductOptionService _productOptionService;
    private readonly IScheduleService _scheduleService;
    private readonly AtraccionDbContext _db;

    public ProductOptionController(
        IProductOptionService productOptionService,
        IScheduleService scheduleService,
        AtraccionDbContext db)
    {
        _productOptionService = productOptionService;
        _scheduleService = scheduleService;
        _db = db;
    }

    // ── MODALIDADES ───────────────────────────────────────────────

    /// <summary>
    /// Lista todas las modalidades (opciones) de una atracción.
    /// Ej: GET /api/v1/ProductOption/by-attraction/{attractionId}
    /// </summary>
    [HttpGet("by-attraction/{attractionId:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ProductOptionDetailResponse>>> GetByAttraction(Guid attractionId)
    {
        var result = await _productOptionService.GetByAttractionAsync(attractionId);
        return Ok(result);
    }

    /// <summary>
    /// Obtiene el detalle de una modalidad específica por su ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProductOptionDetailResponse>> GetById(Guid id)
    {
        var result = await _productOptionService.GetByIdAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// Crea una nueva modalidad bajo una atracción existente.
    /// Permite agregar una opción sin tener que recrear la atracción completa.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<object>> Create([FromBody] CreateProductOptionRequest request)
    {
        var id = await _productOptionService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id }, new { id, message = "Modalidad creada con éxito." });
    }

    /// <summary>
    /// Edita los datos generales de una modalidad existente.
    /// No afecta sus precios, horarios ni plantillas (se gestionan por separado).
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateProductOptionRequest request)
    {
        await _productOptionService.UpdateAsync(id, request);
        return Ok(new { message = "Modalidad actualizada con éxito." });
    }

    /// <summary>
    /// Activa o desactiva una modalidad sin eliminarla.
    /// Los slots y reservas futuras no se ven afectados.
    /// </summary>
    [HttpPatch("{id:guid}/toggle")]
    public async Task<ActionResult> Toggle(Guid id, [FromBody] bool isActive)
    {
        var result = await _productOptionService.ToggleActiveAsync(id, isActive);
        if (!result) return NotFound();
        var estado = isActive ? "activada" : "desactivada";
        return Ok(new { message = $"Modalidad {estado}." });
    }

    /// <summary>
    /// Elimina una modalidad permanentemente.
    /// Falla si la modalidad tiene reservas activas.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var result = await _productOptionService.DeleteAsync(id);
        if (!result) return NotFound();
        return Ok(new { message = "Modalidad eliminada." });
    }

    // ── HORARIOS DE LA MODALIDAD ──────────────────────────────────

    /// <summary>
    /// Lista todas las plantillas de horario de una modalidad.
    /// Cada plantilla define en qué días y a qué horas opera este producto.
    /// </summary>
    [HttpGet("{productId:guid}/schedules")]
    [AllowAnonymous]
    public async Task<ActionResult<List<ScheduleTemplateResponse>>> GetSchedules(Guid productId)
    {
        var result = await _scheduleService.GetByProductAsync(productId);
        return Ok(result);
    }

    /// <summary>
    /// Crea una nueva plantilla de horario para esta modalidad Y genera automáticamente
    /// todos los slots de disponibilidad en el rango de fechas indicado.
    /// Una sola llamada es suficiente — no hace falta llamar a /generate después.
    /// </summary>
    [HttpPost("{productId:guid}/schedules")]
    public async Task<ActionResult<object>> CreateSchedule(
        Guid productId,
        [FromBody] CreateScheduleTemplateRequest request)
    {
        request.ProductId = productId;
        var id = await _scheduleService.CreateAsync(request);
        return Ok(new { id, message = "Plantilla creada y slots de disponibilidad generados correctamente." });
    }

    /// <summary>
    /// [OPCIONAL] Re-genera los slots de disponibilidad para una plantilla ya existente
    /// en un rango de fechas específico. Útil si editaste la plantilla y necesitas
    /// actualizar los slots sin borrar reservas activas.
    /// </summary>
    [HttpPost("{productId:guid}/schedules/{templateId:guid}/generate")]
    public async Task<ActionResult<object>> GenerateSlots(
        Guid productId,
        Guid templateId,
        [FromBody] GenerateSlotsRequest request)
    {
        var result = await _scheduleService.GenerateSlotsAsync(templateId, request);
        return Ok(new
        {
            result.SlotsCreated,
            result.SlotsSkipped,
            result.SlotsDeleted,
            message = $"{result.SlotsCreated} slots regenerados exitosamente."
        });
    }

    // ── GESTIÓN DE SLOTS GENERADOS ────────────────────────────────

    /// <summary>
    /// Lista todos los slots de disponibilidad de una modalidad con fechas, horas,
    /// capacidad y si tiene reservas activas (HasBookings).
    /// Acepta filtros opcionales por fromDate y toDate.
    /// El Frontend usa este listado para mostrar el calendario y permitir eliminar slots.
    /// </summary>
    [HttpGet("{productId:guid}/slots")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<AvailabilitySlotResponse>>> GetSlots(
        Guid productId,
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate)
    {
        var query = _db.AvailabilitySlots
            .Where(s => s.ProductId == productId);

        if (fromDate.HasValue) query = query.Where(s => s.SlotDate >= fromDate.Value);
        if (toDate.HasValue)   query = query.Where(s => s.SlotDate <= toDate.Value);

        var slots = await query
            .OrderBy(s => s.SlotDate)
            .ThenBy(s => s.StartTime)
            .ToListAsync();

        // Obtener IDs con reservas activas en una sola consulta (sin N+1)
        var slotIds = slots.Select(s => s.Id).ToList();
        var bookedSlotIds = await _db.Bookings
            .Where(b => slotIds.Contains(b.SlotId) && b.StatusId != 4)
            .Select(b => b.SlotId)
            .Distinct()
            .ToListAsync();
        var bookedSet = bookedSlotIds.ToHashSet();

        var result = slots.Select(s => new AvailabilitySlotResponse
        {
            Id                = s.Id,
            ProductId         = s.ProductId,
            SlotDate          = s.SlotDate,
            StartTime         = s.StartTime,
            EndTime           = s.EndTime,
            CapacityTotal     = s.CapacityTotal,
            CapacityAvailable = s.CapacityAvailable,
            IsActive          = s.IsActive,
            HasBookings       = bookedSet.Contains(s.Id),
            Notes             = s.Notes
        });

        return Ok(result);
    }

    /// <summary>
    /// Elimina un slot individual.
    /// Rechaza la operación si el slot tiene reservas activas.
    /// </summary>
    [HttpDelete("{productId:guid}/slots/{slotId:guid}")]
    public async Task<ActionResult> DeleteSlot(Guid productId, Guid slotId)
    {
        var deleted = await _scheduleService.DeleteSingleSlotAsync(slotId);
        if (!deleted) return NotFound(new { message = "Slot no encontrado." });
        return Ok(new { message = "Slot eliminado correctamente." });
    }

    /// <summary>
    /// Elimina varios slots a la vez por filtros. 
    /// Los slots con reservas activas NO se eliminan (quedan en slotsSkipped).
    /// Ejemplos:
    ///   - Un día exacto:   { "exactDate": "2026-05-15" }
    ///   - Un rango:        { "fromDate": "2026-05-01", "toDate": "2026-05-31" }
    ///   - Solo sábados:    { "fromDate": "2026-05-01", "toDate": "2026-12-31", "dayOfWeek": 6 }
    /// dayOfWeek: 0=Dom, 1=Lun, 2=Mar, 3=Mié, 4=Jue, 5=Vie, 6=Sáb
    /// </summary>
    [HttpDelete("{productId:guid}/slots")]
    public async Task<ActionResult> DeleteSlotsBulk(Guid productId, [FromBody] DeleteSlotsRequest request)
    {
        request.ProductId = productId;
        var result = await _scheduleService.DeleteSlotsAsync(request);
        return Ok(new
        {
            result.SlotsDeleted,
            result.SlotsSkipped,
            message = $"{result.SlotsDeleted} slots eliminados. {result.SlotsSkipped} omitidos por tener reservas activas."
        });
    }
}
