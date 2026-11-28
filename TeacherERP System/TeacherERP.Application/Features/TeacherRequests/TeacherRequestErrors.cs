// جديد — TeacherERP.Application/Features/TeacherRequests/TeacherRequestErrors.cs
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.TeacherRequests
{
    public static class TeacherRequestErrors
    {
        // نفس كود وجملة التسجيل عمداً: لا نفرّق بين "لم يُرسل" و"الإيميل غير مسموح" فلا نكشف الإيميلات المسجّلة
        public static readonly Error SubmissionExpired = new("Otp.Expired", "The request has expired or the email is incorrect. Please submit your request again.");

        public static readonly Error NotFound = new("TeacherRequest.NotFound", "The request was not found.");
        public static readonly Error NotPending = new("TeacherRequest.NotPending", "This request has already been reviewed.");
        public static readonly Error Expired = new("TeacherRequest.Expired", "This request has expired. The applicant must submit a new one.");
        public static readonly Error ReviewerNotAllowed = new("TeacherRequest.ReviewerNotAllowed", "Your account is not allowed to review teacher requests.");

        public static readonly Error QuotaReached = new(
            "TeacherRequest.QuotaReached",
            $"You can admit only {TeacherRequestPolicy.MaxApprovalsPerWindow} teacher every {TeacherRequestPolicy.ApprovalWindowDays} days.");
    }
}
