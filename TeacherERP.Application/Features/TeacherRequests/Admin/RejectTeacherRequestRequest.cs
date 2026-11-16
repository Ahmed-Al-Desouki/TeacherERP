// جديد — TeacherERP.Application/Features/TeacherRequests/Admin/RejectTeacherRequestRequest.cs
namespace TeacherERP.Application.Features.TeacherRequests.Admin
{
    // جسم طلب الرفض من مدير النظام (المعرّف يأتي من الـ URL)
    public record RejectTeacherRequestRequest(string? Reason);
}
