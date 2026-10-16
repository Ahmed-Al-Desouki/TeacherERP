using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Groups.GetGroups
{
    // مجموعات المدرس الحالي: المدرس يرى الكل، والمساعد المخصصة له فقط
    public record GetGroupsQuery : IRequest<Result<List<GroupDto>>>;
}