// جديد — TeacherERP.Application/Features/Enrollment/EnrollmentDtos.cs
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Features.Enrollment
{
    // طلب الطالب (انضمام أو نقل) كما يراه هو
    public record EnrollmentRequestDto(
        Guid Id,
        Guid GroupId,
        string GroupName,
        Guid TeacherId,
        EnrollmentRequestType Type,
        EnrollmentRequestStatus Status,
        DateTime RequestedAt,
        string? DecisionNote)
    {
        public static EnrollmentRequestDto From(MyRequestItem item)
            => new(item.RequestId, item.GroupId, item.GroupName, item.TeacherId, item.Type, item.Status, item.RequestedAt, item.DecisionNote);
    }

    public record MyEnrollmentDto(
        Guid EnrollmentId, Guid GroupId, string GroupName, Guid TeacherId, string TeacherName, string? Subject, DateTime EnrolledAt)
    {
        public static MyEnrollmentDto From(MyEnrollmentItem item)
            => new(item.EnrollmentId, item.GroupId, item.GroupName, item.TeacherId, item.TeacherName, item.Subject, item.EnrolledAt);
    }

    public record MyEnrollmentsDto(List<MyEnrollmentDto> Enrollments, List<EnrollmentRequestDto> Requests);

    // طلب كما يراه الموظف (CurrentGroupName للنقل: المجموعة الحالية للطالب)
    public record EnrollmentRequestListDto(
        Guid Id,
        Guid StudentId,
        string StudentName,
        Guid GroupId,
        string GroupName,
        EnrollmentRequestType Type,
        EnrollmentRequestStatus Status,
        DateTime RequestedAt,
        string? CurrentGroupName)
    {
        public static EnrollmentRequestListDto From(EnrollmentRequestListItem item)
            => new(item.Id, item.StudentId, item.StudentName, item.GroupId, item.GroupName, item.Type, item.Status, item.RequestedAt, item.CurrentGroupName);
    }
}