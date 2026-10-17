using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Groups
{
    public static class GroupErrors
    {
        // نفس الخطأ لمجموعة غير موجودة أو غير مخصصة لك
        public static readonly Error NotFound = new("Group.NotFound", "The group was not found.");
        public static readonly Error NameTaken = new("Group.NameTaken", "You already have a group with this name.");
        public static readonly Error Inactive = new("Group.Inactive", "The group is archived.");
        public static readonly Error CenterNotAllowed = new("Group.CenterNotAllowed", "This center is not available for this teacher.");
    }
}