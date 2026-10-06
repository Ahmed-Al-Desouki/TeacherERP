using MediatR;
using Hangfire;
using Microsoft.Extensions.Caching.Memory;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Users.Commands.CreateStudent
{
    public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, Result<string>>
    {
        private readonly IIdentityService _identityService;
        private readonly IMemoryCache _cache; // استخدمنا الكاش لحفظ البيانات مؤقتاً
        private readonly IBackgroundJobClient _backgroundJobClient;

        public CreateStudentCommandHandler(
            IIdentityService identityService,
            IMemoryCache cache,
            IBackgroundJobClient backgroundJobClient)
        {
            _identityService = identityService;
            _cache = cache;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task<Result<string>> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
        {
            // 1. التحقق من عدم وجود الإيميل في قاعدة البيانات مسبقاً
            var emailExistsResult = await _identityService.CheckEmailExistsAsync(request.Email, cancellationToken);
            if (emailExistsResult.Value)
            {
                return Result<string>.Failure(new Error("User.EmailExists", "This email is already registered."));
            }

            // 2. توليد OTP عشوائي (6 أرقام)
            var otpCode = new Random().Next(100000, 999999).ToString();

            // 3. تخزين بيانات التسجيل بالكامل في الـ Cache لمدة 10 دقائق
            var cacheData = new CachedRegistrationData
            {
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Password = request.Password,
                OtpCode = otpCode
            };

            _cache.Set($"Reg_{request.Email}", cacheData, TimeSpan.FromMinutes(10));

            // 4. إرسال الإيميل في الخلفية
            _backgroundJobClient.Enqueue<IEmailService>(emailService =>
                emailService.SendOtpEmailAsync(request.Email, otpCode));

            // 5. إرجاع الإيميل بنجاح للخطوة التالية
            return Result<string>.Success(request.Email);
        }
    }


}