// جديد — TeacherERP.Application/Features/TeacherRequests/TeacherRequestRules.cs
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.TeacherRequests
{
    // قاعدة واحدة لمن يجوز مراجعته (المعاينة والقبول والرفض)
    public static class TeacherRequestRules
    {
        public static Error? CheckReviewable(TeacherAccessRequest request, DateTime nowUtc)
        {
            return request.GetEffectiveStatus(nowUtc) switch
            {
                TeacherAccessRequestStatus.Pending => null,
                TeacherAccessRequestStatus.Expired => TeacherRequestErrors.Expired,
                _ => TeacherRequestErrors.NotPending
            };
        }
    }
}
