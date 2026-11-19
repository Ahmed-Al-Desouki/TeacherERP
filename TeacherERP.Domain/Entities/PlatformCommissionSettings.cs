namespace TeacherERP.Domain.Entities;

/// <summary>Global, administrator-managed commission amounts used for future commission accruals.</summary>
public sealed class PlatformCommissionSettings : BaseEntity
{
    public static readonly Guid GlobalSettingsId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    public const decimal DefaultPerSessionAmount = 1m;
    public const decimal DefaultMonthlySubscriptionAmount = 5m;
    public const decimal MaximumCommissionAmount = 100_000m;

    public decimal PerSessionAmount { get; private set; }
    public decimal MonthlySubscriptionAmount { get; private set; }
    public Guid? UpdatedByUserId { get; private set; }
    public DateTime? UpdatedAtUtc { get; private set; }

    private PlatformCommissionSettings() { }

    public static PlatformCommissionSettings CreateDefault() => new()
    {
        Id = GlobalSettingsId,
        PerSessionAmount = DefaultPerSessionAmount,
        MonthlySubscriptionAmount = DefaultMonthlySubscriptionAmount
    };

    public bool Update(decimal perSessionAmount, decimal monthlySubscriptionAmount,
        Guid administratorId, DateTime updatedAtUtc)
    {
        ValidateAmount(perSessionAmount, nameof(perSessionAmount));
        ValidateAmount(monthlySubscriptionAmount, nameof(monthlySubscriptionAmount));
        if (administratorId == Guid.Empty)
            throw new ArgumentException("An administrator is required.", nameof(administratorId));

        if (PerSessionAmount == perSessionAmount && MonthlySubscriptionAmount == monthlySubscriptionAmount)
            return false;

        PerSessionAmount = perSessionAmount;
        MonthlySubscriptionAmount = monthlySubscriptionAmount;
        UpdatedByUserId = administratorId;
        UpdatedAtUtc = DateTime.SpecifyKind(updatedAtUtc, DateTimeKind.Utc);
        MarkAsUpdated();
        return true;
    }

    private static void ValidateAmount(decimal amount, string name)
    {
        if (amount < 0 || amount > MaximumCommissionAmount || decimal.Round(amount, 2) != amount)
            throw new ArgumentOutOfRangeException(name,
                $"Commission must be between 0 and {MaximumCommissionAmount:0.00} with at most two decimal places.");
    }
}
