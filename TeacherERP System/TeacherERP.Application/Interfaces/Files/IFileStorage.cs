// جديد — TeacherERP.Application/Interfaces/IFileStorage.cs
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Interfaces
{
    public record StoredFile(string Url, string PublicId);
    public record FileStorageResource(string PublicId);
    public record FileStorageResourcePage(IReadOnlyList<FileStorageResource> Resources, string? NextCursor);

    // تخزين الملفات الخارجي (Cloudinary حالياً). التنفيذ في Infrastructure ويمكن استبداله بدون المساس بالـ Handlers
    public interface IFileStorage
    {
        // الملف يُرفع باسم عشوائي داخل folder. الـ Stream لازم يكون عند بدايته
        Task<Result<StoredFile>> UploadAsync(Stream content, string fileName, string folder, CancellationToken cancellationToken);

        // حذف ملف غير موجود يُعتبر نجاحاً
        Task<Result<bool>> DeleteAsync(string publicId, CancellationToken cancellationToken);

        // صفحة محدودة من ملفات التطبيق فقط؛ يستخدمها فاحص الملفات اليتيمة بعد مقارنة المراجع في قاعدة البيانات.
        Task<FileStorageResourcePage> ListApplicationFilesAsync(string? nextCursor, int pageSize, CancellationToken cancellationToken);
    }
}
