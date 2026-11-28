// جديد — TeacherERP.Application/Features/Users/Commands/Auth/ActivateAccount/ActivateAccountCommandHandler.cs
using MediatR;
using TeacherERP.Application.Features.Users.Commands.Auth.Login;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Users.Commands.Auth.ActivateAccount
{
    public class ActivateAccountCommandHandler : IRequestHandler<ActivateAccountCommand, Result<AuthResponseDto>>
    {
        private readonly IOtpService _otpService;
        private readonly IUserAccountService _userAccountService;
        private readonly ISessionManager _sessionManager;

        public ActivateAccountCommandHandler(
            IOtpService otpService,
            IUserAccountService userAccountService,
            ISessionManager sessionManager)
        {
            _otpService = otpService;
            _userAccountService = userAccountService;
            _sessionManager = sessionManager;
        }

        public async Task<Result<AuthResponseDto>> Handle(ActivateAccountCommand request, CancellationToken cancellationToken)
        {
            // 1. الكود أولاً (يشمل حد المحاولات والقفل). الكود لا يوجد إلا لو الـ Login أصدره لحساب موجود غير مفعّل
            var otpResult = await _otpService.VerifyAsync(OtpPurpose.Activation, request.Email, request.OtpCode, cancellationToken);
            if (otpResult.IsFailure)
            {
                return Result<AuthResponseDto>.Failure(otpResult.Error);
            }

            // 2. الحساب لازم يكون موجوداً وغير مفعّل
            var userResult = await _userAccountService.FindByEmailAsync(request.Email, cancellationToken);
            var user = userResult.Value;
            if (user is null || user.EmailConfirmed)
            {
                return Result<AuthResponseDto>.Failure(new Error("Otp.Invalid", "The verification code is invalid."));
            }

            if (user.Status != StudentStatus.Active)
            {
                return Result<AuthResponseDto>.Failure(new Error("Auth.AccountSuspended", "This account is suspended or blacklisted."));
            }

            // 3. إثبات ملكية الإيميل
            var confirmResult = await _userAccountService.ConfirmEmailAsync(user, cancellationToken);
            if (confirmResult.IsFailure)
            {
                return Result<AuthResponseDto>.Failure(confirmResult.Error);
            }

            // 4. إصدار الجلسة، مع علامة عرض "تغيير الباسورد؟" (يقدر المساعد يتخطاها)
            var response = await _sessionManager.IssueAsync(user, request.DeviceId, request.DeviceInfo, cancellationToken);
            return Result<AuthResponseDto>.Success(response with { RequiresPasswordSetup = true });
        }
    }
}