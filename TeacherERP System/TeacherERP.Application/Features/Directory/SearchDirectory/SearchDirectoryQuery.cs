// موجود (تعديل) — TeacherERP.Application/Features/Directory/SearchDirectory/SearchDirectoryQuery.cs
using MediatR;
using TeacherERP.Application.Common;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Directory.SearchDirectory
{
    // بحث باسم المدرس أو السنتر أو المادة (حرفان على الأقل). الصفحات على مستوى السناتر. لا تظهر إلا المدرسون الذين اختاروا الظهور
    public record SearchDirectoryQuery(string Term, int Page = 1, int PageSize = PageRequest.DefaultPageSize)
        : IRequest<Result<PagedResult<DirectoryCenterDto>>>, IPagedRequest;
}
