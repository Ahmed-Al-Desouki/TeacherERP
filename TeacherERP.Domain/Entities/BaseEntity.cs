using System;
using System.Collections.Generic;
using System.Text;

namespace TeacherERP.Domain.Entities
{
    /// <summary>
    /// الفئة الأساسية (Base Entity) التي ترث منها جميع كيانات النظام (باستثناء ApplicationUser لأنه يرث من IdentityUser).
    /// توفر هذه الفئة الخصائص المشتركة لتجنب تكرار الكود وتطبيق مبدأ DRY.
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>المعرف الفريد للكيان (Primary Key)</summary>
        public Guid Id { get; protected set; }

        /// <summary>تاريخ ووقت إنشاء السجل في قاعدة البيانات</summary>
        public DateTime CreatedAt { get; protected set; }

        /// <summary>تاريخ ووقت آخر تعديل على السجل (يمكن أن يكون Null إذا لم يتم تعديله)</summary>
        public DateTime? UpdatedAt { get; protected set; }

        // يمكن إضافة خصائص أخرى مثل IsDeleted لتطبيق الـ Soft Delete لاحقاً إذا رغبت.

        protected BaseEntity()
        {
            // توليد آلي لتوفير الكود في الـ Factory Methods
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// دالة يتم استدعاؤها عند تحديث الكيان لتسجيل وقت التعديل.
        /// (في الغالب نقوم بتحديث هذا الحقل أوتوماتيكياً عبر الـ Interceptors أو SaveChanges في EF Core)
        /// </summary>
        public void MarkAsUpdated()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
