// جديد — TeacherERP.Application/Features/Students/StudentReasonRequest.cs
namespace TeacherERP.Application.Features.Students
{
    // جسم الطلب لفك الإيقاف والحظر وفك الحظر (الـ Id من المسار). السبب إجباري ويُسجَّل في الـ Audit
    public record StudentReasonRequest(string Reason);
}