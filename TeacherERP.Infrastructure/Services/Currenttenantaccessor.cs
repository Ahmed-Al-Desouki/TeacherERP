using TeacherERP.Application.Interfaces;

namespace TeacherERP.Infrastructure.Services
{
    // Scoped: نسخة لكل طلب أو Job. لا يعتمد على الداتابيز عمداً (AppDbContext سيعتمد عليه في الفلتر العام).
    public class CurrentTenantAccessor : ICurrentTenantAccessor
    {
        public TenantContext? Current { get; private set; }

        public Guid TenantId => Current?.TenantId ?? Guid.Empty;

        public void Set(TenantContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            // التكرار بنفس القيمة مقبول (أكثر من Policy في نفس الطلب)، أما تغيير السنتر فخطأ برمجي خطير
            if (Current is not null && Current != context)
            {
                throw new InvalidOperationException("The current center was already set for this scope and cannot be changed.");
            }

            Current = context;
        }
    }
}