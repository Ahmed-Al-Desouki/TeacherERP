// جديد — TeacherERP.Application/Files/FileStorageErrors.cs
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Files
{
    public static class FileStorageErrors
    {
        public static readonly Error NotConfigured = new("FileStorage.NotConfigured", "File storage is not configured.");
        public static readonly Error UploadFailed = new("FileStorage.UploadFailed", "The file could not be uploaded. Please try again.");
        public static readonly Error DeleteFailed = new("FileStorage.DeleteFailed", "The file could not be deleted.");
    }
}