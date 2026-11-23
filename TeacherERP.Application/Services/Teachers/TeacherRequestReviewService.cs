// موجود (تعديل) — TeacherERP.Application/Teachers/TeacherRequestReviewService.cs
using System.Data;
using TeacherERP.Application.Features.Administration;
using TeacherERP.Application.Features.TeacherRequests;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Teachers
{
    public class TeacherRequestReviewService : ITeacherRequestReviewService
    {
        private readonly ITeacherAccessRequestRepository _requestRepository;
        private readonly ITeacherProvisioningService _provisioningService;
        private readonly ITenantRepository _tenantRepository;
        private readonly IAuditRepository _auditRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public TeacherRequestReviewService(
            ITeacherAccessRequestRepository requestRepository,
            ITeacherProvisioningService provisioningService,
            ITenantRepository tenantRepository,
            IAuditRepository auditRepository,
            IUnitOfWork unitOfWork,
            TimeProvider timeProvider)
        {
            _requestRepository = requestRepository;
            _provisioningService = provisioningService;
            _tenantRepository = tenantRepository;
            _auditRepository = auditRepository;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        public async Task<Result<CreatedTeacherDto>> ApproveAsync(
            TeacherAccessRequest request, Guid reviewerUserId, ReviewerKind reviewer, CancellationToken cancellationToken)
        {
            var now = _timeProvider.GetUtcNow().UtcDateTime;

            var reviewable = TeacherRequestRules.CheckReviewable(request, now);
            if (reviewable is not null)
            {
                return Result<CreatedTeacherDto>.Failure(reviewable);
            }

            // المدرس: لازم مساحته نشطة، وعليه سقف إدخال (مدير النظام مستثنى). المساحة هي التي يُسجَّل فيها التدقيق
            Guid? auditTenantId = null;
            if (reviewer == ReviewerKind.Teacher)
            {
                var reviewerTenant = await GetActiveReviewerTenantAsync(reviewerUserId, cancellationToken);
                if (reviewerTenant is null)
                {
                    return Result<CreatedTeacherDto>.Failure(TeacherRequestErrors.ReviewerNotAllowed);
                }

                var since = now.AddDays(-TeacherRequestPolicy.ApprovalWindowDays);
                var approved = await _requestRepository.CountApprovedByReviewerSinceAsync(reviewerUserId, since, cancellationToken);
                if (approved >= TeacherRequestPolicy.MaxApprovalsPerWindow)
                {
                    return Result<CreatedTeacherDto>.Failure(TeacherRequestErrors.QuotaReached);
                }

                auditTenantId = reviewerTenant.Id;
            }

            // الإنشاء وتحديث حالة الطلب والتدقيق كلها في Transaction واحدة: إما تتم كلها أو لا يبقى شيء.
            // فلا يبقى حساب منشأ وطلب معلّق (إعادة المحاولة تفشل بـ EmailInUse للأبد)
            try
            {
                return await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    // الإنشاء الفعلي (حساب + مساحة + إيميل الباسورد المؤقت) من الخدمة المشتركة نفسها مع مدير النظام.
                    // تنضم لهذه الـ Transaction، فلو فشلت نتراجع نحن عن كل ما كتبته
                    var provisioned = await _provisioningService.ProvisionAsync(
                        request.FullName, request.Email, request.TimeZoneId, cancellationToken);
                    if (provisioned.IsFailure)
                    {
                        throw new ProvisioningRejectedException(provisioned.Error);
                    }

                    var created = provisioned.Value!;

                    // المادة التي كتبها صاحب الطلب تُضبط على مساحته الجديدة
                    var newTenant = await _tenantRepository.GetByOwnerIdAsync(created.UserId, cancellationToken);
                    newTenant?.UpdateProfile(newTenant.Name, request.Subject);

                    request.Approve(reviewerUserId, now, created.UserId);

                    if (auditTenantId is { } tenantId)
                    {
                        _auditRepository.Add(AuditLog.Create(tenantId, reviewerUserId, AuditAction.TeacherAdmitted, created.UserId, null));
                    }

                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    return Result<CreatedTeacherDto>.Success(created);
                }, IsolationLevel.ReadCommitted, cancellationToken);
            }
            catch (ProvisioningRejectedException ex)
            {
                return Result<CreatedTeacherDto>.Failure(ex.Error);
            }
        }

        public async Task<Result<bool>> RejectAsync(
            TeacherAccessRequest request, Guid reviewerUserId, ReviewerKind reviewer, string? reason, CancellationToken cancellationToken)
        {
            var now = _timeProvider.GetUtcNow().UtcDateTime;

            var reviewable = TeacherRequestRules.CheckReviewable(request, now);
            if (reviewable is not null)
            {
                return Result<bool>.Failure(reviewable);
            }

            Guid? auditTenantId = null;
            if (reviewer == ReviewerKind.Teacher)
            {
                var reviewerTenant = await GetActiveReviewerTenantAsync(reviewerUserId, cancellationToken);
                if (reviewerTenant is null)
                {
                    return Result<bool>.Failure(TeacherRequestErrors.ReviewerNotAllowed);
                }

                auditTenantId = reviewerTenant.Id;
            }

            request.Reject(reviewerUserId, now, reason);

            if (auditTenantId is { } tenantId)
            {
                _auditRepository.Add(AuditLog.Create(tenantId, reviewerUserId, AuditAction.TeacherRequestRejected, null, request.DecisionNote));
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }

        // مساحة المدرس المراجِع (لكل مدرس مساحة واحدة يملكها)، وnull لو غير موجودة أو موقوفة
        private async Task<Tenant?> GetActiveReviewerTenantAsync(Guid reviewerUserId, CancellationToken cancellationToken)
        {
            var tenant = await _tenantRepository.GetByOwnerIdAsync(reviewerUserId, cancellationToken);
            return tenant is { Status: TenantStatus.Active } ? tenant : null;
        }

        // يُستخدم لإجبار الـ Transaction على التراجع عندما ترفض خدمة الإنشاء (إيميل مستخدم أو فشل الإيميل)
        private sealed class ProvisioningRejectedException : Exception
        {
            public ProvisioningRejectedException(Error error)
            {
                Error = error;
            }

            public Error Error { get; }
        }
    }
}
