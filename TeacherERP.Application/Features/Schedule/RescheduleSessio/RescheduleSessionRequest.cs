namespace TeacherERP.Application.Features.Schedule.RescheduleSession
{
    // جسم الطلب (الـ Id من المسار). المدة اختيارية: لو لم تُرسل تبقى مدة الحصة كما هي
    public record RescheduleSessionRequest(DateTime NewStartTime, int? DurationMinutes);
}