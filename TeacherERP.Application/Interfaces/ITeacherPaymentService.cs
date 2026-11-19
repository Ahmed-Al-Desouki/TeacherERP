using TeacherERP.Domain.Entities;
using TeacherERP.Application.Common;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Interfaces;

public interface ITeacherPaymentService
{
    Task<TeacherPaymentSettingsDto?> GetSettingsAsync(Guid tenantId, CancellationToken cancellationToken);
    Task<Result<TeacherPaymentSettingsDto>> UpdateSettingsAsync(Guid tenantId, Guid ownerId, string instaPayLink,
        Stream? qrImage, string? fileName, CancellationToken cancellationToken);
    Task<Result<StudentPaymentDetails>> GetStudentPaymentDetailsAsync(
        Guid tenantId, Guid studentId, Guid sessionId, CancellationToken cancellationToken);
    Task<Result<Guid>> SubmitReceiptAsync(Guid tenantId, Guid studentId, Guid sessionId, PaymentType type,
        decimal amount, Stream file, string fileName, CancellationToken cancellationToken);
    Task<PagedResult<PendingExternalPaymentItem>> GetPendingPaymentsPageAsync(
        Guid tenantId, PageRequest page, CancellationToken cancellationToken);
    Task<Result<CashPaymentResult>> ApproveAsync(
        Guid tenantId, Guid reviewerId, Guid transactionId, CancellationToken cancellationToken);
    Task<Result<bool>> RejectAsync(
        Guid tenantId, Guid reviewerId, Guid transactionId, CancellationToken cancellationToken);
}
