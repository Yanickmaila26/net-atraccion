using Servicio.Atraccion.Business.DTOs.Payment;
using Servicio.Atraccion.Business.Interfaces;
using Servicio.Atraccion.DataManagement.Interfaces;

namespace Servicio.Atraccion.Business.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentDataService _paymentData;

    public PaymentService(IPaymentDataService paymentData)
    {
        _paymentData = paymentData;
    }

    public async Task<IEnumerable<PaymentResponse>> GetPaymentsByBookingIdAsync(Guid bookingId)
    {
        var payments = await _paymentData.GetPaymentsByBookingIdAsync(bookingId);
        return payments.Select(MapToResponse);
    }

    public async Task<PaymentResponse?> GetPaymentByIdAsync(Guid id)
    {
        var payment = await _paymentData.GetPaymentByIdAsync(id);
        return payment == null ? null : MapToResponse(payment);
    }

    public async Task<Guid> CreatePaymentAsync(CreatePaymentRequest request)
    {
        var payment = new DataAccess.Entities.Payment
        {
            Id = Guid.NewGuid(),
            BookingId = request.BookingId,
            PaymentMethodId = request.PaymentMethodId,
            Amount = request.Amount,
            CurrencyCode = request.CurrencyCode,
            TransactionExternalId = request.TransactionExternalId,
            StatusId = request.StatusId ?? 1, // Usa el que viene o Pending por defecto
            PaidAt = request.StatusId == 2 ? DateTime.UtcNow : null,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return await _paymentData.AddPaymentAsync(payment);
    }

    public async Task<bool> UpdatePaymentStatusAsync(Guid id, UpdatePaymentStatusRequest request)
    {
        var payment = await _paymentData.GetPaymentByIdAsync(id);
        if (payment == null) return false;

        payment.StatusId = request.StatusId;
        
        if (!string.IsNullOrEmpty(request.TransactionExternalId))
            payment.TransactionExternalId = request.TransactionExternalId;
            
        if (!string.IsNullOrEmpty(request.GatewayResponse))
            payment.GatewayResponse = request.GatewayResponse;

        if (request.StatusId == 2) // Asumiendo 2 = Completed/Paid
        {
            payment.PaidAt = DateTime.UtcNow;
        }

        payment.UpdatedAt = DateTime.UtcNow;

        return await _paymentData.UpdatePaymentAsync(payment);
    }

    public async Task<bool> ProcessRefundAsync(Guid id, ProcessRefundRequest request)
    {
        var payment = await _paymentData.GetPaymentByIdAsync(id);
        if (payment == null) return false;

        // Validar que el pago esté en un estado reembolsable (ej. Completed)
        if (payment.StatusId != 2) // Assuming 2 = Paid
            throw new InvalidOperationException("Solo los pagos completados pueden ser reembolsados.");

        payment.StatusId = 4; // Asumiendo 4 = Refunded
        payment.RefundedAt = DateTime.UtcNow;
        payment.RefundReason = request.RefundReason;
        payment.UpdatedAt = DateTime.UtcNow;

        return await _paymentData.UpdatePaymentAsync(payment);
    }

    private static PaymentResponse MapToResponse(DataAccess.Entities.Payment p)
    {
        return new PaymentResponse
        {
            Id = p.Id,
            BookingId = p.BookingId,
            TransactionExternalId = p.TransactionExternalId,
            PaymentMethodId = p.PaymentMethodId,
            PaymentMethodName = p.PaymentMethod?.Name ?? string.Empty,
            StatusId = p.StatusId,
            StatusName = p.Status?.Name ?? string.Empty,
            Amount = p.Amount,
            CurrencyCode = p.CurrencyCode,
            PaidAt = p.PaidAt,
            RefundedAt = p.RefundedAt,
            RefundReason = p.RefundReason,
            CreatedAt = p.CreatedAt
        };
    }
}
