// جديد — TeacherERP.Application/Features/SessionContent/GetSessionAttachments/GetSessionAttachmentsQuery.cs
using MediatR;
using TeacherERP.Application.Common;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.SessionContent.GetSessionAttachments
{
    // ملفات حصة (للموظفين، وللطالب المسؤول عن مجموعتها). الأحدث أولاً، والنوع اختياري
    public record GetSessionAttachmentsQuery(
        Guid SessionId,
        SessionAttachmentKind? Kind,
        int Page = 1,
        int PageSize = PageRequest.DefaultPageSize)
        : IRequest<Result<PagedResult<SessionAttachmentDto>>>, IPagedRequest;
}