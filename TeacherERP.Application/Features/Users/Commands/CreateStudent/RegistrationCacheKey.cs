// جديد — TeacherERP.Application/Features/Users/Commands/CreateStudent/RegistrationCacheKey.cs
namespace TeacherERP.Application.Features.Users.Commands.CreateStudent
{
    // مفتاح واحد لبيانات التسجيل المعلقة، بنفس تطبيع الـ OtpService (بدون مسافات وبحروف صغيرة)
    // فلا يفرق الإيميل بين Ali@Mail.com وali@mail.com بين خطوة التسجيل وخطوة التحقق
    public static class RegistrationCacheKey
    {
        public static string For(string email) => $"Reg_{email.Trim().ToLowerInvariant()}";
    }
}
