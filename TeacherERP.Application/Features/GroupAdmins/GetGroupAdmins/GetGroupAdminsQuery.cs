// جديد — TeacherERP.Application/Features/GroupAdmins/GetGroupAdmins/GetGroupAdminsQuery.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.GroupAdmins.GetGroupAdmins
{
    // الطلاب المسؤولون عن مجموعة (حدّهم 2 لكل مجموعة فالقائمة بلا صفحات)
    public record GetGroupAdminsQuery(Guid GroupId) : IRequest<Result<List<GroupAdminDto>>>;
}