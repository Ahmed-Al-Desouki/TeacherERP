// جديد — TeacherERP.Application/Features/GroupAdmins/GroupAdminErrors.cs
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.GroupAdmins
{
    public static class GroupAdminErrors
    {
        public static readonly Error NotAllowed = new("GroupAdmin.NotAllowed", "You are not allowed to manage this group.");
        public static readonly Error NotEnrolled = new("GroupAdmin.NotEnrolled", "The student is not enrolled in this group.");
        public static readonly Error StudentNotActive = new("GroupAdmin.StudentNotActive", "The student is suspended or blacklisted.");
        public static readonly Error AlreadyAdmin = new("GroupAdmin.AlreadyAdmin", "The student is already an admin of this group.");
        public static readonly Error NotAdmin = new("GroupAdmin.NotAdmin", "The student is not an admin of this group.");
        public static readonly Error LimitReached = new("GroupAdmin.LimitReached", "A group can have at most 2 student admins.");
        public static readonly Error CannotRecordSelf = new("GroupAdmin.CannotRecordSelf", "You cannot record your own attendance.");
        public static readonly Error RequestNotFound = new("GroupAdmin.RequestNotFound", "The request was not found.");
        public static readonly Error RequestNotPending = new("GroupAdmin.RequestNotPending", "The request was already handled.");
        public static readonly Error RequestAlreadyPending = new("GroupAdmin.RequestAlreadyPending", "There is already a pending request for this student in this group.");
    }
}