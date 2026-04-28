using Microsoft.EntityFrameworkCore;
using Servicio.Atraccion.Business.DTOs.Schedule;
using Servicio.Atraccion.Business.Exceptions;
using Servicio.Atraccion.DataAccess.Context;
using Servicio.Atraccion.DataAccess.Entities;
using Servicio.Atraccion.DataAccess.Repositories.Interfaces;

namespace Servicio.Atraccion.Business.Services;

public interface IScheduleService
{
    Task<List<ScheduleTemplateResponse>> GetByProductAsync(Guid productId);
    Task<ScheduleTemplateResponse> GetByIdAsync(Guid templateId);
    Task<Guid> CreateAsync(CreateScheduleTemplateRequest request);
    Task UpdateAsync(Guid templateId, UpdateScheduleTemplateRequest request);
    Task DeleteTemplateAsync(Guid templateId);

    // Generación masiva de slots
    Task<GenerateSlotsResult> GenerateSlotsAsync(Guid templateId, GenerateSlotsRequest request);

    // Eliminación flexible de slots
    Task<DeleteSlotsResult> DeleteSlotsAsync(DeleteSlotsRequest request);
    Task<bool> DeleteSingleSlotAsync(Guid slotId);
}

public class ScheduleService : IScheduleService
{
    private readonly IUnitOfWork _uow;
    private readonly AtraccionDbContext _db;

    public ScheduleService(IUnitOfWork uow, AtraccionDbContext db)
    {
        _uow = uow;
        _db = db;
    }

    // ── CONSULTAS ────────────────────────────────────────────

    public async Task<List<ScheduleTemplateResponse>> GetByProductAsync(Guid productId)
    {
        var templates = await _db.ScheduleTemplates
            .Include(t => t.Times)
            .Where(t => t.ProductId == productId)
            .OrderBy(t => t.ValidFrom)
            .ToListAsync();

        return templates.Select(MapToResponse).ToList();
    }

    public async Task<ScheduleTemplateResponse> GetByIdAsync(Guid templateId)
    {
        var template = await _db.ScheduleTemplates
            .Include(t => t.Times)
            .FirstOrDefaultAsync(t => t.Id == templateId)
            ?? throw new NotFoundException("PlantillaHorario", templateId);

        return MapToResponse(template);
    }

    // ── CREACIÓN ─────────────────────────────────────────────

    public async Task<Guid> CreateAsync(CreateScheduleTemplateRequest request)
    {
        if (!request.Times.Any())
            throw new BusinessException("Debes agregar al menos un horario de salida a la plantilla.");

        var template = new ProductScheduleTemplate
        {
            Id               = Guid.NewGuid(),
            ProductId        = request.ProductId,
            Name             = request.Name,
            Monday           = request.Monday,
            Tuesday          = request.Tuesday,
            Wednesday        = request.Wednesday,
            Thursday         = request.Thursday,
            Friday           = request.Friday,
            Saturday         = request.Saturday,
            Sunday           = request.Sunday,
            ValidFrom        = request.ValidFrom,
            ValidTo          = request.ValidTo,
            DefaultCapacity  = request.DefaultCapacity,
            Notes            = request.Notes,
            IsActive         = true
        };

        foreach (var t in request.Times)
            template.Times.Add(new ProductScheduleTime
            {
                StartTime        = t.StartTime,
                EndTime          = t.EndTime,
                CapacityOverride = t.CapacityOverride,
                SortOrder        = t.SortOrder
            });

        await _uow.ScheduleTemplates.AddAsync(template);

        // ── Generación automática de slots en el mismo acto ──────────
        var validTo = template.ValidTo ?? template.ValidFrom.AddMonths(3);
        for (var date = template.ValidFrom; date <= validTo; date = date.AddDays(1))
        {
            if (!IsActiveDay(template, date.DayOfWeek)) continue;

            foreach (var time in template.Times.OrderBy(t => t.SortOrder))
            {
                var capacity = time.CapacityOverride ?? template.DefaultCapacity;
                _db.AvailabilitySlots.Add(new AvailabilitySlot
                {
                    Id                = Guid.NewGuid(),
                    ProductId         = template.ProductId,
                    SlotDate          = date,
                    StartTime         = time.StartTime,
                    EndTime           = time.EndTime,
                    CapacityTotal     = capacity,
                    CapacityAvailable = capacity,
                    IsActive          = true
                });
            }
        }

        await _uow.CompleteAsync();
        return template.Id;
    }

    // ── EDICIÓN ──────────────────────────────────────────────

    public async Task UpdateAsync(Guid templateId, UpdateScheduleTemplateRequest request)
    {
        var template = await _db.ScheduleTemplates
            .Include(t => t.Times)
            .FirstOrDefaultAsync(t => t.Id == templateId)
            ?? throw new NotFoundException("PlantillaHorario", templateId);

        template.Name            = request.Name;
        template.Monday          = request.Monday;
        template.Tuesday         = request.Tuesday;
        template.Wednesday       = request.Wednesday;
        template.Thursday        = request.Thursday;
        template.Friday          = request.Friday;
        template.Saturday        = request.Saturday;
        template.Sunday          = request.Sunday;
        template.ValidFrom       = request.ValidFrom;
        template.ValidTo         = request.ValidTo;
        template.DefaultCapacity = request.DefaultCapacity;
        template.Notes           = request.Notes;
        template.IsActive        = request.IsActive;
        template.UpdatedAt       = DateTime.UtcNow;

        // Reemplazar horarios: borrar los viejos y agregar los nuevos
        _db.ScheduleTimes.RemoveRange(template.Times);
        template.Times.Clear();

        foreach (var t in request.Times)
            template.Times.Add(new ProductScheduleTime
            {
                StartTime        = t.StartTime,
                EndTime          = t.EndTime,
                CapacityOverride = t.CapacityOverride,
                SortOrder        = t.SortOrder
            });

        await _uow.CompleteAsync();
    }

    // ── ELIMINACIÓN DE PLANTILLA ─────────────────────────────

    public async Task DeleteTemplateAsync(Guid templateId)
    {
        var template = await _db.ScheduleTemplates
            .Include(t => t.Times)
            .FirstOrDefaultAsync(t => t.Id == templateId)
            ?? throw new NotFoundException("PlantillaHorario", templateId);

        _db.ScheduleTimes.RemoveRange(template.Times);
        _db.ScheduleTemplates.Remove(template);
        await _uow.CompleteAsync();
    }

    // ── GENERACIÓN MASIVA DE SLOTS ────────────────────────────

    public async Task<GenerateSlotsResult> GenerateSlotsAsync(Guid templateId, GenerateSlotsRequest request)
    {
        var template = await _db.ScheduleTemplates
            .Include(t => t.Times)
            .FirstOrDefaultAsync(t => t.Id == templateId)
            ?? throw new NotFoundException("PlantillaHorario", templateId);

        if (!template.Times.Any())
            throw new BusinessException("La plantilla no tiene horarios definidos. Agrega al menos uno antes de generar slots.");

        var result = new GenerateSlotsResult();

        // Si overwrite, borrar slots sin reservas activas dentro del rango
        if (request.OverwriteExisting)
        {
            var toDelete = await _db.AvailabilitySlots
                .Where(s => s.ProductId == template.ProductId
                         && s.SlotDate >= request.FromDate
                         && s.SlotDate <= request.ToDate
                         && !_db.Bookings.Any(b => b.SlotId == s.Id))
                .ToListAsync();

            _db.AvailabilitySlots.RemoveRange(toDelete);
            result.SlotsDeleted = toDelete.Count;
        }

        // Obtener slots ya existentes para evitar duplicados
        var existingSlots = await _db.AvailabilitySlots
            .Where(s => s.ProductId == template.ProductId
                     && s.SlotDate >= request.FromDate
                     && s.SlotDate <= request.ToDate)
            .Select(s => new { s.SlotDate, s.StartTime })
            .ToListAsync();

        var existingSet = existingSlots
            .Select(s => (s.SlotDate, s.StartTime))
            .ToHashSet();

        // Iterar cada día del rango
        for (var date = request.FromDate; date <= request.ToDate; date = date.AddDays(1))
        {
            if (!IsActiveDay(template, date.DayOfWeek))
            {
                result.SlotsSkipped++;
                continue;
            }

            // Verificar que el día esté dentro del período de vigencia del template
            if (date < template.ValidFrom || (template.ValidTo.HasValue && date > template.ValidTo.Value))
            {
                result.SlotsSkipped++;
                continue;
            }

            foreach (var time in template.Times.OrderBy(t => t.SortOrder))
            {
                if (existingSet.Contains((date, time.StartTime)))
                {
                    result.SlotsSkipped++;
                    continue;
                }

                var capacity = time.CapacityOverride ?? template.DefaultCapacity;

                _db.AvailabilitySlots.Add(new AvailabilitySlot
                {
                    Id                 = Guid.NewGuid(),
                    ProductId          = template.ProductId,
                    SlotDate           = date,
                    StartTime          = time.StartTime,
                    EndTime            = time.EndTime,
                    CapacityTotal      = capacity,
                    CapacityAvailable  = capacity,
                    IsActive           = true
                });

                existingSet.Add((date, time.StartTime));
                result.SlotsCreated++;
            }
        }

        await _uow.CompleteAsync();
        return result;
    }

    // ── ELIMINACIÓN FLEXIBLE DE SLOTS ────────────────────────

    public async Task<DeleteSlotsResult> DeleteSlotsAsync(DeleteSlotsRequest request)
    {
        var query = _db.AvailabilitySlots
            .Where(s => s.ProductId == request.ProductId);

        // Filtro por fecha exacta
        if (request.ExactDate.HasValue)
        {
            query = query.Where(s => s.SlotDate == request.ExactDate.Value);
        }
        else
        {
            // Filtro por rango
            if (request.FromDate.HasValue)
                query = query.Where(s => s.SlotDate >= request.FromDate.Value);
            if (request.ToDate.HasValue)
                query = query.Where(s => s.SlotDate <= request.ToDate.Value);

            // Filtro por día de semana
            if (request.DayOfWeek.HasValue)
                query = query.Where(s => (int)s.SlotDate.DayOfWeek == request.DayOfWeek.Value);
        }

        var slots = await query.ToListAsync();
        var result = new DeleteSlotsResult();

        foreach (var slot in slots)
        {
            // No borrar si tiene reservas activas
            var hasBooking = await _db.Bookings
                .AnyAsync(b => b.SlotId == slot.Id && b.StatusId != 4); // 4 = Cancelled

            if (hasBooking)
            {
                result.SlotsSkipped++;
                continue;
            }

            _db.AvailabilitySlots.Remove(slot);
            result.SlotsDeleted++;
        }

        await _uow.CompleteAsync();
        return result;
    }

    public async Task<bool> DeleteSingleSlotAsync(Guid slotId)
    {
        var slot = await _db.AvailabilitySlots.FindAsync(slotId);
        if (slot == null) return false;

        var hasBooking = await _db.Bookings
            .AnyAsync(b => b.SlotId == slotId && b.StatusId != 4);

        if (hasBooking)
            throw new BusinessException("No se puede eliminar este horario porque tiene reservas activas.");

        _db.AvailabilitySlots.Remove(slot);
        await _uow.CompleteAsync();
        return true;
    }

    // ── HELPERS ──────────────────────────────────────────────

    private static bool IsActiveDay(ProductScheduleTemplate t, DayOfWeek day) => day switch
    {
        DayOfWeek.Monday    => t.Monday,
        DayOfWeek.Tuesday   => t.Tuesday,
        DayOfWeek.Wednesday => t.Wednesday,
        DayOfWeek.Thursday  => t.Thursday,
        DayOfWeek.Friday    => t.Friday,
        DayOfWeek.Saturday  => t.Saturday,
        DayOfWeek.Sunday    => t.Sunday,
        _                   => false
    };

    private static ScheduleTemplateResponse MapToResponse(ProductScheduleTemplate t) => new()
    {
        Id               = t.Id,
        ProductId        = t.ProductId,
        Name             = t.Name,
        Monday           = t.Monday,
        Tuesday          = t.Tuesday,
        Wednesday        = t.Wednesday,
        Thursday         = t.Thursday,
        Friday           = t.Friday,
        Saturday         = t.Saturday,
        Sunday           = t.Sunday,
        ValidFrom        = t.ValidFrom,
        ValidTo          = t.ValidTo,
        DefaultCapacity  = t.DefaultCapacity,
        IsActive         = t.IsActive,
        Notes            = t.Notes,
        Times            = t.Times
            .OrderBy(x => x.SortOrder)
            .Select(x => new ScheduleTimeResponse
            {
                Id               = x.Id,
                StartTime        = x.StartTime,
                EndTime          = x.EndTime,
                CapacityOverride = x.CapacityOverride,
                SortOrder        = x.SortOrder
            }).ToList()
    };
}
