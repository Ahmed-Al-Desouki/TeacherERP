namespace TeacherERP.Application.Interfaces;

public interface IOrphanedFileCleanupJob
{
    Task RunPageAsync(string? nextCursor = null);
    Task DeleteCandidateAsync(string publicId);
}
