// جديد — TeacherERP.Application/Features/Students/GetStudents/GetStudentsQuery.cs
using MediatR;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Students.GetStudents
{
    // طلاب المدرس الحالي (للمدرس والمساعد). تصفية اختيارية بحالة العضوية أو المجموعة
    public record GetStudentsQuery(MembershipStatus? Status, Guid? GroupId) : IRequest<Result<List<StudentListDto>>>;
}