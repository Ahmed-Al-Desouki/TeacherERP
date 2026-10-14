namespace TeacherERP.API.Security
{
    public static class AuthPolicies
    {
        /// <summary>يمنع الطالب صاحب البروفايل الناقص (المدرس والمساعد مستثنيان)</summary>
        public const string CompletedProfile = "CompletedProfile";

        /// <summary>يحدد السنتر الحالي (X-Center-Id أو تلقائياً لو عضوية واحدة) ويتأكد من عضوية المستخدم فيه</summary>
        public const string TenantMember = "TenantMember";

        /// <summary>نفس TenantMember لكن للمدرس (Owner) أو المساعد (Assistant) فقط</summary>
        public const string TenantStaff = "TenantStaff";

        /// <summary>نفس TenantMember لكن للمدرس (Owner) فقط</summary>
        public const string TenantOwner = "TenantOwner";
    }
}