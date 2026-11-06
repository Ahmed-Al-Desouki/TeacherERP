// جديد — TeacherERP.Application/Features/SessionContent/SessionContentErrors.cs
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.SessionContent
{
    public static class SessionContentErrors
    {
        // نفس الخطأ لملف غير موجود أو تابع لمدرس آخر
        public static readonly Error AttachmentNotFound = new("SessionContent.AttachmentNotFound", "The file was not found.");
        public static readonly Error FileRequired = new("SessionContent.FileRequired", "A file is required.");
        public static readonly Error KindRequired = new("SessionContent.KindRequired", "The file kind is required.");
        public static readonly Error EmptyFile = new("SessionContent.EmptyFile", "The file is empty.");
        public static readonly Error FileTooLarge = new("SessionContent.FileTooLarge", "The file must not exceed 10 MB.");
        public static readonly Error UnsupportedType = new("SessionContent.UnsupportedType", "Only JPEG, PNG, WEBP and PDF files are allowed.");
        public static readonly Error LimitReached = new("SessionContent.LimitReached", "A session can have at most 30 files.");
        public static readonly Error OutsideAdminWindow = new("SessionContent.OutsideAdminWindow", "Student admins can manage files only around the session time.");
        public static readonly Error OnlyOwnFiles = new("SessionContent.OnlyOwnFiles", "You can delete only the files you uploaded.");
    }
}