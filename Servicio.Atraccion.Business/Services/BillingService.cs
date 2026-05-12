using Microsoft.EntityFrameworkCore;
using Servicio.Atraccion.Business.DTOs.Billing;
using Servicio.Atraccion.Business.Interfaces;
using Servicio.Atraccion.DataAccess.Common;
using Servicio.Atraccion.DataAccess.Repositories.Interfaces;

namespace Servicio.Atraccion.Business.Services;

public class BillingService : IBillingService
{
    private readonly IUnitOfWork _uow;

    public BillingService(IUnitOfWork uow)
    {
        _uow = uow;
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
}
