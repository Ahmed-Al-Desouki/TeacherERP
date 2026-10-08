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
            // 1. التحقق من عدم وجود الإيميل في قاعدة البيانات مسبقاً
            var emailExistsResult = await _userAccountService.CheckEmailExistsAsync(request.Email, cancellationToken);
            if (emailExistsResult.Value)
            {
                return Result<string>.Failure(new Error("User.EmailExists", "This email is already registered."));
            }

            // 2. توليد الـ OTP (كود آمن + فترة انتظار بين الطلبات + قفل بعد المحاولات الخاطئة)
            // نولّد الكود أولاً حتى لا نستبدل بيانات تسجيل معلقة إذا كان الطلب مرفوضاً
            var otpResult = await _otpService.GenerateAsync(OtpPurpose.Registration, request.Email, cancellationToken);
            if (otpResult.IsFailure)
            {
                return Result<string>.Failure(otpResult.Error);
            }

            // 3. تخزين بيانات التسجيل في الـ Cache لمدة 10 دقائق
            var cacheData = new CachedRegistrationData
            {
                FullName = request.FullName,
                Email = request.Email,
                Password = request.Password
            };

            _cache.Set($"Reg_{request.Email}", cacheData, TimeSpan.FromMinutes(10));

            // 4. إرسال الإيميل في الخلفية
            var otpCode = otpResult.Value!.Code;
            _backgroundJobClient.Enqueue<IEmailService>(emailService =>
                emailService.SendOtpEmailAsync(request.Email, otpCode));

            // 5. إرجاع الإيميل بنجاح للخطوة التالية
            return Result<string>.Success(request.Email);
        }
    }
}