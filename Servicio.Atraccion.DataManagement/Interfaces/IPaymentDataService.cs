using Servicio.Atraccion.DataAccess.Entities;

namespace Servicio.Atraccion.DataManagement.Interfaces;

public interface IPaymentDataService
{
    Task<IEnumerable<Payment>> GetPaymentsByBookingIdAsync(Guid bookingId);
    Task<Payment?> GetPaymentByIdAsync(Guid id);
    Task<Guid> AddPaymentAsync(Payment payment);
    Task<bool> UpdatePaymentAsync(Payment payment);
}
