namespace TeacherERP.API.Security
{
    public static class AuthPolicies
    {
        /// <summary>يمنع الطالب صاحب البروفايل الناقص (المدرس والمساعد مستثنيان)</summary>
        public const string CompletedProfile = "CompletedProfile";
    }
}