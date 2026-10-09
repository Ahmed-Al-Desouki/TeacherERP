using MediatR;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;
using TeacherERP.Domain.ValueObjects;

namespace TeacherERP.Application.Features.Profile.CompleteProfile
{
    // استبدال كامل: Gender وDateOfBirth اختياريان (لو لم يُرسلا يُمسحان)
    public record CompleteProfileCommand(
        string PhoneNumber,
        string GuardianName,
        string GuardianPhoneNumber,
        GuardianRelation GuardianRelation,
        Gender? Gender,
        DateOnly? DateOfBirth) : IRequest<Result<ProfileDto>>;
}