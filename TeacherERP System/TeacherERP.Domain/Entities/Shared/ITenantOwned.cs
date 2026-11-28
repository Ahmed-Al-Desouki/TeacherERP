using System;

namespace TeacherERP.Domain.Entities
{
    /// <summary>كيان تابع لسنتر (مدرس) معيّن: يُطبَّق عليه عزل البيانات تلقائياً</summary>
    public interface ITenantOwned
    {
        Guid TenantId { get; }
    }
}