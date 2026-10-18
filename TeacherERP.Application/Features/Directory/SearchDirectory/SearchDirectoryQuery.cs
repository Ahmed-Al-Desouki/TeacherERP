// جديد — TeacherERP.Application/Features/Directory/SearchDirectory/SearchDirectoryQuery.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Directory.SearchDirectory
{
    // بحث باسم المدرس أو السنتر أو المادة (حرفان على الأقل). لا تظهر إلا المدرسون الذين اختاروا الظهور
    public record SearchDirectoryQuery(string Term) : IRequest<Result<List<DirectoryCenterDto>>>;
}