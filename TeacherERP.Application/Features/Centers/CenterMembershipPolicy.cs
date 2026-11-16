namespace TeacherERP.Application.Features.Centers
{
    public static class CenterMembershipPolicy
    {
        public const int MaxPendingRequestsPerTeacher = 3;
        public static readonly TimeSpan RequestReminderInterval = TimeSpan.FromHours(24);
        public static readonly TimeSpan InvitationLifetime = TimeSpan.FromHours(72);
        public static readonly TimeSpan ClosedDataRetention = TimeSpan.FromDays(365);
    }
}
