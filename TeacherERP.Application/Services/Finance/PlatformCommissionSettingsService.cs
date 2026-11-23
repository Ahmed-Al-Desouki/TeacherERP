using System.Data;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Finance;

public sealed class PlatformCommissionSettingsService : IPlatformCommissionSettingsService
{
    private readonly IFinancialLedgerRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;

    public PlatformCommissionSettingsService(IFinancialLedgerRepository repository,
        IUnitOfWork unitOfWork, TimeProvider timeProvider)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
    }

    public async Task<PlatformCommissionSettingsDto> GetAsync(CancellationToken cancellationToken)
    {
        var settings = await _repository.GetPlatformCommissionSettingsAsync(cancellationToken)
            ?? PlatformCommissionSettings.CreateDefault();
        return ToDto(settings);
    }

    public Task<Result<PlatformCommissionSettingsDto>> UpdateAsync(Guid administratorId,
        decimal perSessionAmount, decimal monthlySubscriptionAmount, CancellationToken cancellationToken)
    {
        if (administratorId == Guid.Empty || !IsValidAmount(perSessionAmount)
            || !IsValidAmount(monthlySubscriptionAmount))
            return Task.FromResult(Result<PlatformCommissionSettingsDto>.Failure(CommissionSettingsErrors.Invalid));

        var updatedAtUtc = _timeProvider.GetUtcNow().UtcDateTime;
        return _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var settings = await _repository.GetPlatformCommissionSettingsAsync(cancellationToken);
            if (settings is null)
            {
                settings = PlatformCommissionSettings.CreateDefault();
                _repository.AddPlatformCommissionSettings(settings);
            }

            settings.Update(perSessionAmount, monthlySubscriptionAmount, administratorId, updatedAtUtc);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<PlatformCommissionSettingsDto>.Success(ToDto(settings));
        }, IsolationLevel.Serializable, cancellationToken);
    }

    private static bool IsValidAmount(decimal amount) => amount is >= 0
        and <= PlatformCommissionSettings.MaximumCommissionAmount && decimal.Round(amount, 2) == amount;

    private static PlatformCommissionSettingsDto ToDto(PlatformCommissionSettings settings) =>
        new(settings.PerSessionAmount, settings.MonthlySubscriptionAmount,
            settings.UpdatedByUserId, settings.UpdatedAtUtc);
}

public static class CommissionSettingsErrors
{
    public static readonly Error Invalid = new("Finance.InvalidCommissionSettings",
        $"Commission amounts must be between 0 and {PlatformCommissionSettings.MaximumCommissionAmount:0.00} EGP with at most two decimal places.");
}
