using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.ValueObjects;

namespace TeacherERP.Application.Features.Students.GetStudentDetails;

public sealed record StudentActiveGroupDto(Guid GroupId, string GroupName, DateTime EnrolledAt);

public sealed record StudentDetailsDto(
    Guid StudentId,
    string FullName,
    string? ProfileImageUrl,
    string? Email,
    string? PhoneNumber,
    Gender? Gender,
    DateOnly? DateOfBirth,
    MembershipStatus MembershipStatus,
    int ConsecutiveAbsences,
    string? GuardianName,
    string? GuardianPhoneNumber,
    GuardianRelation? GuardianRelation,
    int ProfileCompletionPercentage,
    IReadOnlyList<StudentActiveGroupDto> ActiveGroups)
{
    public static StudentDetailsDto From(StudentDetailsItem item) => new(
        item.StudentId,
        item.FullName,
        item.ProfileImageUrl,
        item.Email,
        item.PhoneNumber,
        item.Gender,
        item.DateOfBirth,
        item.MembershipStatus,
        item.ConsecutiveAbsences,
        item.GuardianName,
        item.GuardianPhoneNumber,
        item.GuardianRelation,
        item.ProfileCompletionPercentage,
        item.ActiveGroups
            .Select(group => new StudentActiveGroupDto(group.GroupId, group.GroupName, group.EnrolledAt))
            .ToArray());
}
