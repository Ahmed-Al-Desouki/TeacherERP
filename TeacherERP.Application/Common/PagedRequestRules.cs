// جديد — TeacherERP.Application/Common/PagedRequestRules.cs
using FluentValidation;

namespace TeacherERP.Application.Common
{
    // قواعد الصفحات في مكان واحد، تُضمَّن في validator أي استعلام قائمة (Include)
    public class PagedRequestRules<T> : AbstractValidator<T> where T : IPagedRequest
    {
        private const int MaxPage = 100_000;

        public PagedRequestRules()
        {
            RuleFor(x => x.Page)
                .InclusiveBetween(1, MaxPage).WithMessage($"Page must be between 1 and {MaxPage}.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, PageRequest.MaxPageSize)
                .WithMessage($"Page size must be between 1 and {PageRequest.MaxPageSize}.");
        }
    }
}