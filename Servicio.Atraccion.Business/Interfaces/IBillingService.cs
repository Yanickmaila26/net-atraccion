using Servicio.Atraccion.Business.DTOs.Billing;
using Servicio.Atraccion.DataAccess.Common;

namespace Servicio.Atraccion.Business.Interfaces;

public interface IBillingService
{
    Task<PagedResult<InvoiceSummaryResponse>> GetManagementInvoicesAsync(QueryFilters filters);
    Task<InvoiceFullResponse?> GetInvoiceByIdAsync(Guid id);
    Task CrearFacturaAsync(DataAccess.Entities.Booking booking, DTOs.Booking.BillingInfo? billing, List<DataAccess.Entities.BookingDetail> bookingDetails);
}
