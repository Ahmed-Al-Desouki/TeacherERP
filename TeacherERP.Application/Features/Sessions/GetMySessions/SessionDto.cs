namespace TeacherERP.Application.Features.Sessions.GetMySessions
{
    // الأوقات UTC (تظهر في الـ JSON بالـ Z) والتطبيق يحوّلها لتوقيت الجهاز
    public record SessionDto(Guid Id, string DeviceName, DateTime CreatedAt, DateTime LastActiveAt, bool IsCurrent);
}