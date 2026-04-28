using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servicio.Atraccion.Business.DTOs.Schedule;
using Servicio.Atraccion.Business.Services;

namespace Servicio.Atraccion.API.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "Admin,Partner")]
public class ScheduleController : ControllerBase
{
    private readonly IScheduleService _scheduleService;

    public ScheduleController(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    // ── PLANTILLAS ────────────────────────────────────────────

    /// <summary>
    /// Obtiene todas las plantillas de horario de un producto/modalidad.
    /// </summary>
    [HttpGet("product/{productId:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<List<ScheduleTemplateResponse>>> GetByProduct(Guid productId)
    {
        var result = await _scheduleService.GetByProductAsync(productId);
        return Ok(result);
    }

    /// <summary>
    /// Obtiene el detalle de una plantilla de horario por su ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ScheduleTemplateResponse>> GetById(Guid id)
    {
        var result = await _scheduleService.GetByIdAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// Crea una nueva plantilla de horario con sus horas de salida.
    /// Ejemplo: Lunes a Viernes, válido de mayo a diciembre, con salidas a las 9:00, 10:30, 12:00 y 13:00.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateScheduleTemplateRequest request)
    {
        var id = await _scheduleService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>
    /// Edita una plantilla existente. Reemplaza completamente sus horarios.
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateScheduleTemplateRequest request)
    {
        await _scheduleService.UpdateAsync(id, request);
        return Ok(new { message = "Plantilla de horario actualizada." });
    }

    /// <summary>
    /// Elimina una plantilla de horario y todos sus tiempos definidos.
    /// No afecta los AvailabilitySlots ya generados.
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteTemplate(Guid id)
    {
        await _scheduleService.DeleteTemplateAsync(id);
        return Ok(new { message = "Plantilla eliminada." });
    }

    // ── GENERACIÓN MASIVA DE SLOTS ────────────────────────────

    /// <summary>
    /// Genera automáticamente los AvailabilitySlots para un rango de fechas,
    /// basándose en los días activos y las horas definidas en la plantilla.
    /// Ejemplo: generar todos los slots de mayo y junio con 4 horarios al día.
    /// </summary>
    [HttpPost("{id:guid}/generate")]
    public async Task<ActionResult<GenerateSlotsResult>> GenerateSlots(
        Guid id, [FromBody] GenerateSlotsRequest request)
    {
        var result = await _scheduleService.GenerateSlotsAsync(id, request);
        return Ok(new
        {
            result.SlotsCreated,
            result.SlotsDeleted,
            result.SlotsSkipped,
            message = $"{result.SlotsCreated} slots creados exitosamente."
        });
    }

    // ── ELIMINACIÓN FLEXIBLE DE SLOTS ────────────────────────

    /// <summary>
    /// Elimina un slot individual. Falla si tiene reservas activas.
    /// </summary>
    [HttpDelete("slots/{slotId:guid}")]
    public async Task<ActionResult> DeleteSlot(Guid slotId)
    {
        var deleted = await _scheduleService.DeleteSingleSlotAsync(slotId);
        if (!deleted) return NotFound();
        return Ok(new { message = "Horario eliminado." });
    }

    /// <summary>
    /// Eliminación masiva de slots con múltiples filtros.
    /// Ejemplos de uso:
    ///   - Borrar todos los slots del 15 de mayo: { "exactDate": "2026-05-15" }
    ///   - Borrar todos los sábados de mayo:      { "fromDate": "2026-05-01", "toDate": "2026-05-31", "dayOfWeek": 6 }
    ///   - Borrar todo el mes de junio:           { "fromDate": "2026-06-01", "toDate": "2026-06-30" }
    /// Los slots con reservas activas NO se eliminan (quedan en SlotsSkipped).
    /// dayOfWeek: 0=Domingo, 1=Lunes, 2=Martes, 3=Miércoles, 4=Jueves, 5=Viernes, 6=Sábado
    /// </summary>
    [HttpDelete("slots/bulk")]
    public async Task<ActionResult<DeleteSlotsResult>> DeleteSlotsBulk([FromBody] DeleteSlotsRequest request)
    {
        var result = await _scheduleService.DeleteSlotsAsync(request);
        return Ok(new
        {
            result.SlotsDeleted,
            result.SlotsSkipped,
            message = $"{result.SlotsDeleted} horarios eliminados. {result.SlotsSkipped} omitidos por tener reservas activas."
        });
    }
}
