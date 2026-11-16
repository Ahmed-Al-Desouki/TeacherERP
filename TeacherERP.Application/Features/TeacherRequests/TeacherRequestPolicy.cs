// جديد — TeacherERP.Application/Features/TeacherRequests/TeacherRequestPolicy.cs
namespace TeacherERP.Application.Features.TeacherRequests
{
    // أرقام المسار في مكان واحد
    public static class TeacherRequestPolicy
    {
        /// <summary>مدة صلاحية الطلب والـ QR من لحظة تأكيد الإيميل</summary>
        public static readonly TimeSpan RequestLifetime = TimeSpan.FromHours(72);

        /// <summary>مدة بقاء بيانات الطلب في الكاش قبل تأكيد الـ OTP (نفس عمر الكود)</summary>
        public static readonly TimeSpan PendingDataLifetime = TimeSpan.FromMinutes(10);

        /// <summary>سقف إدخال المدرسين: موافقة واحدة لكل مدرس خلال هذه المدة (مدير النظام مستثنى)</summary>
        public const int ApprovalWindowDays = 30;

        public const int MaxApprovalsPerWindow = 1;
    }
}
