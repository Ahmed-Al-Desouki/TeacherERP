// جديد — TeacherERP.Application/Interfaces/ITeacherRequestReviewService.cs
using TeacherERP.Application.Features.Administration;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Interfaces
{
    /// <summary>من يراجع الطلب: المدرس الذي مسح الـ QR (عليه سقف وتدقيق) أو مدير النظام (مستثنى)</summary>
    public enum ReviewerKind
    {
        Teacher,
        Administrator
    }

    // قبول أو رفض طلب مدرس. مصدر واحد لمسار المدرس ومسار مدير النظام، والإنشاء الفعلي عبر ITeacherProvisioningService
    public interface ITeacherRequestReviewService
    {
        Task<Result<CreatedTeacherDto>> ApproveAsync(
            TeacherAccessRequest request, Guid reviewerUserId, ReviewerKind reviewer, CancellationToken cancellationToken);

        Task<Result<bool>> RejectAsync(
            TeacherAccessRequest request, Guid reviewerUserId, ReviewerKind reviewer, string? reason, CancellationToken cancellationToken);
    }
}
