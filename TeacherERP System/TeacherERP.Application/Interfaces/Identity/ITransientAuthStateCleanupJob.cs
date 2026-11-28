namespace TeacherERP.Application.Interfaces;

public interface ITransientAuthStateCleanupJob
{
    Task RunAsync(CancellationToken cancellationToken = default);
}
