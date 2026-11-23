// جديد — TeacherERP.Application/Files/StoredFileCleanupJob.cs
using TeacherERP.Application.Interfaces;

namespace TeacherERP.Application.Files
{
    public class StoredFileCleanupJob : IStoredFileCleanupJob
    {
        private readonly IFileStorage _fileStorage;

        public StoredFileCleanupJob(IFileStorage fileStorage)
        {
            _fileStorage = fileStorage;
        }

        public async Task DeleteAsync(string publicId)
        {
            var result = await _fileStorage.DeleteAsync(publicId, CancellationToken.None);

            // الاستثناء هو اللي يخلّي Hangfire يعيد المحاولة (نتيجة Failure وحدها لا تكفي)
            if (result.IsFailure)
            {
                throw new InvalidOperationException($"Deleting the stored file failed: {result.Error.Code}");
            }
        }
    }
}