namespace TeacherERP.Application.Features.Sessions.GetMySessions
{
    // الأوقات تظل UTC؛ العميل يستخدم TimeZoneId المعاد مع بيانات المصادقة لتنسيقها.
    public record SessionDto(Guid Id, string DeviceName, DateTime CreatedAt, DateTime LastActiveAt, bool IsCurrent);
}
