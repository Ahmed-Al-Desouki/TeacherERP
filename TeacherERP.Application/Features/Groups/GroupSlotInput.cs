namespace TeacherERP.Application.Features.Groups
{
    // حصة في القالب الأسبوعي: اليوم ووقت البداية (توقيت المدرس) والمدة بالدقائق
    public record GroupSlotInput(DayOfWeek Day, TimeSpan StartTime, int DurationMinutes);
}