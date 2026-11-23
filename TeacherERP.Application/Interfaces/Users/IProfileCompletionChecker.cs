namespace TeacherERP.Application.Interfaces
{
    public interface IProfileCompletionChecker
    {
        Task<bool> IsCompleteAsync(Guid userId, CancellationToken cancellationToken);
    }
}