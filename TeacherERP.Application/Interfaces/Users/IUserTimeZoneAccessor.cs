namespace TeacherERP.Application.Interfaces
{
    public interface IUserTimeZoneAccessor
    {
        // المنطقة الزمنية للطلب الحالي (IANA مثل Africa/Cairo)، أو null إن لم تُرسل أو كانت غير صالحة
        string? TimeZoneId { get; }
    }
}