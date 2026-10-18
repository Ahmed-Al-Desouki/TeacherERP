// موجود (تعديل) — TeacherERP.Infrastructure/Data/UnitOfWork.cs
using System.Data;
using Microsoft.EntityFrameworkCore;
using TeacherERP.Application.Interfaces;

namespace TeacherERP.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<TResult> ExecuteInTransactionAsync<TResult>(
            Func<Task<TResult>> action, IsolationLevel isolationLevel, CancellationToken cancellationToken)
        {
            // أي استثناء قبل الـ Commit يتراجع تلقائياً عند التخلص من الـ Transaction
            await using var transaction = await _context.Database.BeginTransactionAsync(isolationLevel, cancellationToken);

            var result = await action();
            await transaction.CommitAsync(cancellationToken);

            return result;
        }
    }
}