// موجود (تعديل) — TeacherERP.Domain/Shared/RoleNames.cs
namespace TeacherERP.Domain.Shared
{
    // أسماء الأدوار في النظام (مصدر واحد بدل النصوص المكررة)
    public static class RoleNames
    {
        public const string SuperAdmin = "SuperAdmin";
        // مساعد المدرس: يمسك كل مجموعاته (الأدمن ستودنت طالب عادي بعلامة على مجموعته، وليس دوراً هنا)
        public const string Assistant = "Assistant";
        public const string Student = "Student";
    }
}