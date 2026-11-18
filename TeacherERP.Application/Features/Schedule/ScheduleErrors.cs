using TeacherERP.Application.Scheduling;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Schedule
{
    public static class ScheduleErrors
    {
        // غير موجودة وغير تابعة لمدرسك نفس الرد
        public static readonly Error NotFound = new("Session.NotFound", "The session was not found.");
        public static readonly Error InPast = new("Session.InPast", "Only upcoming sessions can be changed.");
        public static readonly Error NewTimeInPast = new("Session.NewTimeInPast", "The new time must be in the future.");
        public static readonly Error Cancelled = new("Session.Cancelled", "The session is cancelled.");
        public static readonly Error TimeConflict = new("Session.TimeConflict", "The time conflicts with another session or group of this teacher.");
        public static readonly Error RoomConflictConfirmationRequired = new(
            "Schedule.RoomConflictConfirmationRequired", "The room is occupied by another teacher. Confirm to proceed.");
        public static readonly Error HasPendingCompensations = new(
            "Session.HasPendingCompensations",
            "This session has pending compensation requests. Approve or reject them before cancelling it.");

        public static Error InvalidRange()
            => new("Schedule.InvalidRange", $"The range must be positive and at most {SchedulePolicy.MaxSessionsQueryDays} days.");
    }
}
