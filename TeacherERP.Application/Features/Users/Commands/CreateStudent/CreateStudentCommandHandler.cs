using MediatR;
using Hangfire;
using Microsoft.Extensions.Caching.Memory;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Users.Commands.CreateStudent
{
    public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, Result<string>>
    {
        private readonly IUserAccountService _userAccountService;
        private readonly IOtpService _otpService;
        private readonly IMemoryCache _cache; // استخدمنا الكاش لحفظ البيانات مؤقتاً
        private readonly IBackgroundJobClient _backgroundJobClient;

        public CreateStudentCommandHandler(
            IUserAccountService userAccountService,
            IOtpService otpService,
            IMemoryCache cache,
            IBackgroundJobClient backgroundJobClient)
        {
            _userAccountService = userAccountService;
            _otpService = otpService;
            _cache = cache;
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

            // 4. تخزين بيانات التسجيل في الـ Cache لمدة 10 دقائق
            var cacheData = new CachedRegistrationData
            {
                FullName = request.FullName,
                Email = request.Email,
                Password = request.Password
            };

            _cache.Set(RegistrationCacheKey.For(request.Email), cacheData, TimeSpan.FromMinutes(10));

            // 5. إرسال الإيميل في الخلفية
            var otpCode = otpResult.Value!.Code;
            _backgroundJobClient.Enqueue<IEmailService>(emailService =>
                emailService.SendOtpEmailAsync(request.Email, otpCode));

            // 6. إرجاع الإيميل بنجاح للخطوة التالية
            return Result<string>.Success(request.Email);
        }
    }
}
