namespace TeacherERP.Application.Scheduling
{
    public static class SchedulePolicy
    {
        /// <summary>نولّد الحصص قدّام بهذه المدة (أيام)، وتتجدد يومياً بالمهمة الدورية</summary>
        public const int GenerationHorizonDays = 28;

        /// <summary>الافتراضي والحد الأقصى لنطاق عرض الحصص (أيام)</summary>
        public const int DefaultSessionsQueryDays = 14;
        public const int MaxSessionsQueryDays = 62;
    }
}