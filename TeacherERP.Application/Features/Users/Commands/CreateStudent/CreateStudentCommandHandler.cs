using MediatR;
using Hangfire;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Users.Commands.CreateStudent
{
    public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, Result<string>>
    {
        private readonly IUserAccountService _userAccountService;
        private readonly IOtpService _otpService;
        private readonly ITransientAuthStateStore _authStateStore;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public CreateStudentCommandHandler(
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

        public async Task<Result<string>> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
        {
            // 1. هل الإيميل مسجّل؟ لا نكشف الإجابة في الرد (منع تعداد الإيميلات المسجّلة)
            var emailExists = (await _userAccountService.CheckEmailExistsAsync(request.Email, cancellationToken)).Value;

            // 2. توليد الـ OTP في الحالتين (كود آمن + فترة انتظار بين الطلبات + قفل بعد المحاولات الخاطئة)
            // فتتساوى حدود الإرسال والانتظار بين الإيميل المسجّل وغير المسجّل، ولا يمكن التفريق بينهما من الردود
            // ونولّد الكود قبل التخزين حتى لا نستبدل بيانات تسجيل معلقة إذا كان الطلب مرفوضاً
            var otpResult = await _otpService.GenerateAsync(OtpPurpose.Registration, request.Email, cancellationToken);
            if (otpResult.IsFailure)
            {
                return Result<string>.Failure(otpResult.Error);
            }

            // 3. إيميل مسجّل: نفس رد النجاح، بدون تخزين بيانات ولا إرسال كود (صاحب الحساب يدخل أو يستعيد باسورده)
            if (emailExists)
            {
                return Result<string>.Success(request.Email);
            }

            var candidate = TeacherERP.Domain.Entities.ApplicationUser.CreateStudent(request.FullName, request.Email);
            var passwordHash = await _userAccountService.HashPasswordAsync(candidate, request.Password, cancellationToken);
            if (passwordHash.IsFailure) return Result<string>.Failure(passwordHash.Error);

            // 4. حفظ مسودة التسجيل عشر دقائق، مع تخزين بصمة كلمة المرور فقط
            var pendingData = new PendingStudentRegistrationData
            {
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = passwordHash.Value!
            };
            await _authStateStore.SavePayloadAsync(OtpPurpose.Registration.ToString(), request.Email, pendingData, TimeSpan.FromMinutes(10), cancellationToken);

            // 5. إرسال الإيميل في الخلفية
            var otpCode = otpResult.Value!.Code;
            _backgroundJobClient.Enqueue<IEmailService>(emailService =>
                emailService.SendOtpEmailAsync(request.Email, otpCode));

            // 6. إرجاع الإيميل بنجاح للخطوة التالية
            return Result<string>.Success(request.Email);
        }
    }
}
