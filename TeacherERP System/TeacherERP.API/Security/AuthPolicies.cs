namespace TeacherERP.API.Security
{
    public static class AuthPolicies
    {
        /// <summary>يمنع الطالب صاحب البروفايل الناقص (المدرس والمساعد مستثنيان)</summary>
        public const string CompletedProfile = "CompletedProfile";

        /// <summary>يحدد السنتر الحالي (X-Teacher-Id أو تلقائياً لو عضوية واحدة) ويتأكد من عضوية المستخدم فيه</summary>
        public const string TenantMember = "TenantMember";

        /// <summary>عضوية الطالب في السنتر الحالي فقط</summary>
        public const string TenantStudent = "TenantStudent";

        /// <summary>نفس TenantMember لكن للمدرس (Owner) أو المساعد (Assistant) فقط</summary>
        public const string TenantStaff = "TenantStaff";

        /// <summary>نفس TenantMember لكن للمدرس (Owner) فقط</summary>
        public const string TenantOwner = "TenantOwner";

        /// <summary>المدرس أو المساعد الذي منحه المدرس صلاحية مالية صريحة</summary>
        public const string TenantFinance = "TenantFinance";

        /// <summary>مدير النظام فقط (دور Administrator). لا يرتبط بأي سنتر، فلا يمر على مصدر السنتر الحالي</summary>
        public const string AdministratorOnly = "AdministratorOnly";

        /// <summary>المدرس (دور SuperAdmin) بلا سنتر حالي: مساحة المدرس المراجِع تُجلب بمالكها، لأن إنشاء مساحة المدرس الجديد لا يجوز تحت سنتر حالي مختلف</summary>
        public const string TeacherOnly = "TeacherOnly";
    }
}
