// جديد — TeacherERP.Application/Features/Directory/SearchDirectory/SearchDirectoryValidator.cs
using FluentValidation;
using TeacherERP.Application.Common;

namespace TeacherERP.Application.Features.Directory.SearchDirectory
{
    // طول النص يُفحص في الـ Handler (بخطأ Directory.InvalidTerm)، وهنا الصفحات فقط
    public class SearchDirectoryValidator : AbstractValidator<SearchDirectoryQuery>
    {
        public SearchDirectoryValidator()
        {
            Include(new PagedRequestRules<SearchDirectoryQuery>());
        }
    }
}
