namespace TeacherERP.Tests.Unit;

// وقت ثابت للاختبارات (بدل الاعتماد على الساعة الحقيقية)
internal sealed class FixedTimeProvider : TimeProvider
{
    private readonly DateTimeOffset _now;

    public FixedTimeProvider(DateTime utcNow)
    {
        _now = new DateTimeOffset(DateTime.SpecifyKind(utcNow, DateTimeKind.Utc));
    }

    public override DateTimeOffset GetUtcNow() => _now;
}
