// جديد — TeacherERP.Application/Features/GroupAdmins/GetMyGroupAdmin/GetMyGroupAdminQuery.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.GroupAdmins.GetMyGroupAdmin
{
    // هل أنا طالب مسؤول عند المدرس الحالي (X-Teacher-Id)؟ وعن أي مجموعة
    public record GetMyGroupAdminQuery : IRequest<Result<GroupAdminMeDto>>;
}