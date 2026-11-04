// جديد — TeacherERP.Infrastructure/Extensions/QueryablePagingExtensions.cs
using Microsoft.EntityFrameworkCore;
using TeacherERP.Application.Common;

namespace TeacherERP.Infrastructure.Extensions
{
    public static class QueryablePagingExtensions
    {
        // الاستعلام لازم يكون مرتّباً ترتيباً ثابتاً (بـ ThenBy على الـ Id) وإلا الصفحات تتكرر أو تضيع منها صفوف
        public static async Task<PagedResult<T>> ToPagedAsync<T>(
            this IQueryable<T> query, PageRequest page, CancellationToken cancellationToken)
        {
            var total = await query.CountAsync(cancellationToken);

            var items = total == 0
                ? new List<T>()
                : await query.Skip(page.Skip).Take(page.PageSize).ToListAsync(cancellationToken);

            return new PagedResult<T>(items, page.Page, page.PageSize, total);
        }
    }
}