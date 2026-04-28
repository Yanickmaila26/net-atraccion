using Servicio.Atraccion.Business.DTOs.Payment;

namespace Servicio.Atraccion.Business.Interfaces;

public interface IPaymentService
{
    Task<IEnumerable<PaymentResponse>> GetPaymentsByBookingIdAsync(Guid bookingId);
    Task<PaymentResponse?> GetPaymentByIdAsync(Guid id);
    Task<Guid> CreatePaymentAsync(CreatePaymentRequest request);
    Task<bool> UpdatePaymentStatusAsync(Guid id, UpdatePaymentStatusRequest request);
    Task<bool> ProcessRefundAsync(Guid id, ProcessRefundRequest request);
}
