using TeacherERP.Domain.Entities;
using TeacherERP.Domain.ValueObjects;

namespace TeacherERP.Application.Features.Profile
{
    public record GuardianDto(string Name, string PhoneNumber, GuardianRelation Relation);

    // MissingFields بنفس أسماء حقول طلب إكمال البروفايل، ليلوّن التطبيق الحقل الناقص
    public record ProfileDto(
        string FullName,
        string Email,
        string? PhoneNumber,
        Gender? Gender,
        DateOnly? DateOfBirth,
        GuardianDto? Guardian,
        bool IsProfileComplete,
        int CompletionPercentage,
        IReadOnlyList<string> MissingFields)
    {
        public static ProfileDto From(ApplicationUser user)
        {
            var guardian = user.Guardian is null
                ? null
                : new GuardianDto(user.Guardian.Name, user.Guardian.PhoneNumber, user.Guardian.Relation);

            return new ProfileDto(
                user.FullName,
                user.Email ?? string.Empty,
                user.PhoneNumber,
                user.Gender,
                user.DateOfBirth,
                guardian,
                user.IsProfileComplete,
                user.ProfileCompletionPercentage,
                user.MissingProfileFields);
        }
    }
}