using FluentValidation;

namespace TeacherERP.Application.Features.Groups
{
    public interface IGroupPricing
    {
        decimal? PerSessionPrice { get; }
        decimal? MonthlyPrice { get; }
    }

    // قواعد مشتركة تمنع اختلاف حدود التسعير بين الإنشاء والتعديل.
    public sealed class GroupPricingRules<T> : AbstractValidator<T> where T : IGroupPricing
    {
        private const decimal MaximumPrice = 9999999999999999.99m;

        public GroupPricingRules()
        {
            RuleFor(x => x.PerSessionPrice)
                .Must(BeValidAmount)
                .WithMessage("Per-session price must be non-negative and support at most two decimal places.");

            RuleFor(x => x.MonthlyPrice)
                .Must(BeValidAmount)
                .WithMessage("Monthly price must be non-negative and support at most two decimal places.");
        }

        private static bool BeValidAmount(decimal? amount)
        {
            if (amount is null)
            {
                return true;
            }

            var value = amount.Value;
            return value >= 0m
                   && value <= MaximumPrice
                   && decimal.Round(value, 2) == value;
        }
    }
}
