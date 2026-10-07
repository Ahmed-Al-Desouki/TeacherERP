using Hangfire;
using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Users.Commands.Auth.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result<bool>>
    {
        private readonly IIdentityService _identityService;
        private readonly IResetGrantService _resetGrantService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public ResetPasswordCommandHandler(
            IIdentityService identityService,
            IResetGrantService resetGrantService,
            IRefreshTokenRepository refreshTokenRepository,
            IUnitOfWork unitOfWork,
            IBackgroundJobClient backgroundJobClient)
        {
            _identityService = identityService;
            _resetGrantService = resetGrantService;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task<Result<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var invalidSession = new Error("Auth.InvalidResetToken", "The reset session is invalid or has expired. Please start again.");

            // 1. التحقق من التصريح
            if (!_resetGrantService.IsValid(request.Email, request.ResetToken))
            {
                return Result<bool>.Failure(invalidSession);
            }

            // 2. جلب المستخدم
            var userResult = await _identityService.FindByEmailAsync(request.Email, cancellationToken);
            var user = userResult.Value;
            if (user is null)
            {
                return Result<bool>.Failure(invalidSession);
            }

            // 3. تغيير الباسورد (يتحقق من سياسة Identity ويحدّث الـ SecurityStamp)
            var resetResult = await _identityService.ResetPasswordAsync(user, request.NewPassword, cancellationToken);
            if (resetResult.IsFailure)
            {
                return Result<bool>.Failure(resetResult.Error);
            }

            // 4. إلغاء كل الجلسات القديمة
            await _refreshTokenRepository.RevokeAllForUserAsync(user.Id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 5. إبطال التصريح (بعد نجاح كل ما سبق، فتبقى إعادة المحاولة ممكنة عند أي خطأ)
            _resetGrantService.Revoke(request.Email);

            // 6. إيميل التنبيه في الخلفية
            var toEmail = user.Email!;
            var changedAt = DateTime.UtcNow;
            _backgroundJobClient.Enqueue<IEmailService>(emailService =>
                emailService.SendPasswordChangedAlertAsync(toEmail, changedAt));

            return Result<bool>.Success(true);
        }
    }
}