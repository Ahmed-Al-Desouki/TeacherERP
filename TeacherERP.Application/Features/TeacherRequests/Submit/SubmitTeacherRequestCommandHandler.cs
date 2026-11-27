// جديد — TeacherERP.Application/Features/TeacherRequests/Submit/SubmitTeacherRequestCommandHandler.cs
using Hangfire;
using MediatR;
using TeacherERP.Application.Features.Administration.CreateTeacher;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.TeacherRequests.Submit
{
    public class SubmitTeacherRequestCommandHandler : IRequestHandler<SubmitTeacherRequestCommand, Result<TeacherRequestOtpDto>>
    {
        private readonly IUserAccountService _userAccountService;
        private readonly IOtpService _otpService;
        private readonly ITransientAuthStateStore _authStateStore;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public SubmitTeacherRequestCommandHandler(
            IUserAccountService userAccountService,
            IOtpService otpService,
            ITransientAuthStateStore authStateStore,
            IBackgroundJobClient backgroundJobClient)
        {
            _userAccountService = userAccountService;
            _otpService = otpService;
            _authStateStore = authStateStore;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task<Result<TeacherRequestOtpDto>> Handle(SubmitTeacherRequestCommand request, CancellationToken cancellationToken)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            var emailExists = (await _userAccountService.CheckEmailExistsAsync(email, cancellationToken)).Value;

            // الكود يُولَّد في الحالتين: الحد الزمني بين الطلبات وسقف الساعة يعملان بنفس الشكل
            // سواء كان الإيميل مسجلاً أم لا، فلا يمكن كشف الإيميلات المسجلة من الردود
            var otpResult = await _otpService.GenerateAsync(OtpPurpose.TeacherRequest, email, cancellationToken);
            if (otpResult.IsFailure)
            {
                return Result<TeacherRequestOtpDto>.Failure(otpResult.Error);
            }

            var otp = otpResult.Value!;
            var response = new TeacherRequestOtpDto(email, otp.ExpiresInSeconds, otp.ResendAvailableInSeconds);

            // إيميل مسجّل: نفس رد النجاح بدون حفظ بيانات ولا إرسال كود (لا تنشأ أي صلة بحساب موجود)
            if (emailExists)
            {
                return Result<TeacherRequestOtpDto>.Success(response);
            }

            await _authStateStore.SavePayloadAsync(
                OtpPurpose.TeacherRequest.ToString(),
                email,
                new PendingTeacherRequestData
                {
                    FullName = request.FullName.Trim(),
                    Email = email,
                    Subject = request.Subject.Trim(),
                    Note = request.Note,
                    TimeZoneId = string.IsNullOrWhiteSpace(request.TimeZoneId)
                        ? CreateTeacherCommandHandler.DefaultTimeZoneId
                        : request.TimeZoneId.Trim()
                },
                TeacherRequestPolicy.PendingDataLifetime,
                cancellationToken);

            var code = otp.Code;
            _backgroundJobClient.Enqueue<IEmailService>(emailService => emailService.SendOtpEmailAsync(email, code));

            return Result<TeacherRequestOtpDto>.Success(response);
        }
    }
}
