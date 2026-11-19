using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Interfaces;

public sealed record PlatformCommissionSettingsDto(decimal PerSessionAmount,
    decimal MonthlySubscriptionAmount, Guid? UpdatedByUserId, DateTime? UpdatedAtUtc);

public interface IPlatformCommissionSettingsService
{
    Task<PlatformCommissionSettingsDto> GetAsync(CancellationToken cancellationToken);
    Task<Result<PlatformCommissionSettingsDto>> UpdateAsync(Guid administratorId,
        decimal perSessionAmount, decimal monthlySubscriptionAmount, CancellationToken cancellationToken);
}
