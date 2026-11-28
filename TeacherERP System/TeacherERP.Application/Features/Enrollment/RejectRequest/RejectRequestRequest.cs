// جديد — TeacherERP.Application/Features/Enrollment/RejectRequest/RejectRequestRequest.cs
namespace TeacherERP.Application.Features.Enrollment.RejectRequest
{
    // جسم الطلب (الـ Id من المسار): سبب الرفض اختياري
    public record RejectRequestRequest(string? Note);
}