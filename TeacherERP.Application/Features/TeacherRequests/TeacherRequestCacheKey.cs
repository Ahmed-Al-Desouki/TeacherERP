// جديد — TeacherERP.Application/Features/TeacherRequests/TeacherRequestCacheKey.cs
namespace TeacherERP.Application.Features.TeacherRequests
{
    // مفتاح بيانات الطلب قبل تأكيد الـ OTP، بنفس تطبيع الـ OtpService
    public static class TeacherRequestCacheKey
    {
        public static string For(string email) => $"TeacherReq_{email.Trim().ToLowerInvariant()}";
    }
}
