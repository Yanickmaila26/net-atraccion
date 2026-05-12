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
[Authorize(Roles = "Admin,Partner")] // Solo personal autorizado
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
}
