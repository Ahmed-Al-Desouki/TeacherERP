// جديد — TeacherERP.Application/Features/Schedule/GetSessions/GetSessionsValidator.cs
using FluentValidation;
using TeacherERP.Application.Common;

namespace TeacherERP.Application.Features.Schedule.GetSessions
{
    // النطاق الزمني يُفحص في الـ Handler (بخطأ Schedule.InvalidRange)، وهنا الصفحات فقط
    public class GetSessionsValidator : AbstractValidator<GetSessionsQuery>
    {
        public GetSessionsValidator()
        {
            Include(new PagedRequestRules<GetSessionsQuery>());
        }
    }
}
