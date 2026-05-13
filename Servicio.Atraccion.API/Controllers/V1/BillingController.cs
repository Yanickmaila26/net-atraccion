using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servicio.Atraccion.Business.DTOs.Billing;
using Servicio.Atraccion.Business.Interfaces;
using Servicio.Atraccion.DataAccess.Common;

namespace Servicio.Atraccion.API.Controllers.V1;

/// <summary>
/// Controlador administrativo para la gestión de facturación.
/// Permite listar y consultar el detalle de las facturas emitidas por el sistema.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "Admin,Partner,Client")] // Permitir a clientes ver sus facturas
public class BillingController : ControllerBase
{
    private readonly IBillingService _billingService;

    public BillingController(IBillingService billingService)
    {
        _billingService = billingService;
    }

    /// <summary>
    /// Obtiene el listado de facturas para el panel administrativo.
    /// </summary>
    [HttpGet("management")]
    public async Task<ActionResult<PagedResult<InvoiceSummaryResponse>>> GetManagementInvoices([FromQuery] QueryFilters filters)
    {
        var result = await _billingService.GetManagementInvoicesAsync(filters);
        return Ok(result);
    }

    /// <summary>
    /// Obtiene el detalle completo de una factura específica.
    /// </summary>
    [HttpGet("management/{id:guid}")]
    public async Task<ActionResult<InvoiceFullResponse>> GetInvoiceDetail(Guid id)
    {
        var result = await _billingService.GetInvoiceByIdAsync(id);
        
        if (result == null)
            return NotFound(new { message = "La factura no existe." });

        return Ok(result);
    }

    /// <summary>
    /// Anula una factura (normalmente vinculada a una reserva cancelada).
    /// </summary>
    [HttpPost("management/{id:guid}/void")]
    public async Task<ActionResult> VoidInvoice(Guid id)
    {
        // Nota: En este contexto usamos el ID de la factura, 
        // pero el servicio espera el ID de la reserva por diseño del monolito actual.
        var invoice = await _billingService.GetInvoiceByIdAsync(id);
        if (invoice == null) return NotFound();

        var result = await _billingService.CancelarFacturaAsync(invoice.BookingId);
        
        if (!result)
            return BadRequest(new { message = "No se pudo anular la factura." });

        return Ok(new { message = "Factura marcada para anulación." });
    }
}
