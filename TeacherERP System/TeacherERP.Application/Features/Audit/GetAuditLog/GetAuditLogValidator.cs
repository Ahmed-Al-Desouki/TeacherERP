// جديد — TeacherERP.Application/Features/Audit/GetAuditLog/GetAuditLogValidator.cs
using FluentValidation;
using TeacherERP.Application.Common;

namespace TeacherERP.Application.Features.Audit.GetAuditLog
{
    public class GetAuditLogValidator : AbstractValidator<GetAuditLogQuery>
    {
        public GetAuditLogValidator()
        {
            Include(new PagedRequestRules<GetAuditLogQuery>());
        }
    }
}
