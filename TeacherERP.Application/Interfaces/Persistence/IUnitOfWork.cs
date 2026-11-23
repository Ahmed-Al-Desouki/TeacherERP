// موجود (تعديل) — TeacherERP.Application/Interfaces/IUnitOfWork.cs
using System.Data;

namespace TeacherERP.Application.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        // عدة حفظات كوحدة واحدة (تتراجع كلها لو حدث استثناء)
        Task<TResult> ExecuteInTransactionAsync<TResult>(
            Func<Task<TResult>> action, IsolationLevel isolationLevel, CancellationToken cancellationToken);
    }
}