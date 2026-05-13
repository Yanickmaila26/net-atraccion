using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Servicio.Atraccion.Business.DTOs.Billing;
using Servicio.Atraccion.Business.Interfaces;
using Servicio.Atraccion.DataAccess.Common;
using Servicio.Atraccion.DataAccess.Repositories.Interfaces;

namespace Servicio.Atraccion.Business.Services;

public class BillingService : IBillingService
{
    private readonly IUnitOfWork _uow;
    private readonly IConfiguration _configuration;

    public BillingService(IUnitOfWork uow, IConfiguration configuration)
    {
        _uow = uow;
        _configuration = configuration;
    }

    public async Task<PagedResult<InvoiceSummaryResponse>> GetManagementInvoicesAsync(QueryFilters filters)
    {
        var query = _uow.Invoices.Query();

        if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
        {
            query = query.Where(i => i.InvoiceNumber.Contains(filters.SearchTerm) || 
                                     i.CustomerName.Contains(filters.SearchTerm) || 
                                     i.TaxId.Contains(filters.SearchTerm));
        }

        var total = await query.CountAsync();
        var items = await query.OrderByDescending(i => i.CreatedAt)
                               .Skip(filters.Offset)
                               .Take(filters.PageSize)
                               .ToListAsync();

        var dtos = items.Select(i => new InvoiceSummaryResponse
        {
            Id = i.Id,
            BookingId = i.BookingId,
            InvoiceNumber = i.InvoiceNumber,
            CustomerName = i.CustomerName,
            TaxId = i.TaxId,
            Total = i.Total,
            Currency = i.CurrencyCode,
            CreatedAt = i.CreatedAt
        }).ToList();

        return new PagedResult<InvoiceSummaryResponse>
        {
            Items = dtos,
            TotalCount = total,
            PageNumber = filters.PageNumber,
            PageSize = filters.PageSize
        };
    }

    public async Task<InvoiceFullResponse?> GetInvoiceByIdAsync(Guid id)
    {
        var invoice = await _uow.Invoices.Query()
            .Include(i => i.Details)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice == null) return null;

        return new InvoiceFullResponse
        {
            Id = invoice.Id,
            BookingId = invoice.BookingId,
            InvoiceNumber = invoice.InvoiceNumber,
            CustomerName = invoice.CustomerName,
            TaxId = invoice.TaxId,
            Email = invoice.Email,
            Address = invoice.Address,
            Subtotal = invoice.Subtotal,
            TaxAmount = invoice.TaxAmount,
            Total = invoice.Total,
            Currency = invoice.CurrencyCode,
            CreatedAt = invoice.CreatedAt,
            Details = invoice.Details.Select(d => new InvoiceDetailResponse
            {
                Id = d.Id,
                Description = d.Description,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
                TaxRate = d.TaxRate,
                TotalItem = d.TotalItem
            }).ToList()
        };
    }

    public async Task CrearFacturaAsync(DataAccess.Entities.Booking booking, DTOs.Booking.BillingInfo? billing, List<DataAccess.Entities.BookingDetail> bookingDetails)
    {
        // Leer tasa de IVA de configuración con fallback seguro
        decimal TAX_RATE = 0.15m;
        try {
            var configValue = _configuration["Billing:TaxRate"];
            if (!string.IsNullOrEmpty(configValue)) {
                TAX_RATE = decimal.Parse(configValue, System.Globalization.CultureInfo.InvariantCulture);
            }
        } catch { /* Usar default 0.15 */ }

        // 1. Determinar datos del cliente (Default a Consumidor Final)
        var customerName = billing?.CustomerName ?? "CONSUMIDOR FINAL";
        var taxId = billing?.TaxId ?? "9999999999";
        var email = billing?.Email;
        var address = billing?.Address ?? "S/N";

        // 2. Calcular impuestos
        decimal subtotal = booking.TotalAmount / (1 + TAX_RATE);
        decimal taxAmount = booking.TotalAmount - subtotal;

        // 3. Crear cabecera de factura
        var invoice = new DataAccess.Entities.Invoice
        {
            Id = Guid.NewGuid(),
            BookingId = booking.Id,
            InvoiceNumber = $"FAC-{booking.PnrCode}", // Simplificado para el demo
            CustomerName = customerName,
            TaxId = taxId,
            Email = email,
            Address = address,
            Subtotal = Math.Round(subtotal, 2),
            TaxAmount = Math.Round(taxAmount, 2),
            Total = booking.TotalAmount,
            CurrencyCode = booking.CurrencyCode,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Invoices.AddAsync(invoice);

        // 4. Crear detalles de factura
        foreach (var detail in bookingDetails)
        {
            decimal itemSubtotal = detail.UnitPrice / (1 + TAX_RATE);
            decimal itemTax = detail.UnitPrice - itemSubtotal;

            var invDetail = new DataAccess.Entities.InvoiceDetail
            {
                Id = Guid.NewGuid(),
                InvoiceId = invoice.Id,
                Description = $"Ticket {detail.FirstName} {detail.LastName}",
                Quantity = detail.Quantity,
                UnitPrice = Math.Round(itemSubtotal, 2),
                TaxRate = TAX_RATE * 100, // Almacenamos 15.00
                TotalItem = detail.UnitPrice * detail.Quantity,
                CreatedAt = DateTime.UtcNow
            };

            await _uow.InvoiceDetails.AddAsync(invDetail);
        }

        await _uow.CompleteAsync();
    }

    public async Task<IEnumerable<InvoiceSummaryResponse>> GetUserInvoicesAsync(Guid userId)
    {
        var items = await _uow.Invoices.Query()
            .Include(i => i.Booking)
            .Where(i => i.Booking.UserId == userId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

        return items.Select(i => new InvoiceSummaryResponse
        {
            Id = i.Id,
            BookingId = i.BookingId,
            InvoiceNumber = i.InvoiceNumber,
            CustomerName = i.CustomerName,
            TaxId = i.TaxId,
            Total = i.Total,
            Currency = i.CurrencyCode,
            CreatedAt = i.CreatedAt
        }).ToList();
    }

    public async Task<bool> CancelarFacturaAsync(Guid bookingId)
    {
        var invoice = await _uow.Invoices.Query()
            .FirstOrDefaultAsync(i => i.BookingId == bookingId);

        if (invoice == null) return false;

        // En un caso real, aquí cambiaríamos el estado de la factura a 'Anulada'
        // Por ahora, solo confirmamos que la factura fue identificada para el proceso.
        // invoice.Status = "Voided"; 
        
        return true;
    }
}
