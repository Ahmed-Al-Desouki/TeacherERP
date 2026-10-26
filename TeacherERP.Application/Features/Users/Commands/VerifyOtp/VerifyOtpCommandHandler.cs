// موجود (تعديل) — TeacherERP.Application/Features/Users/Commands/VerifyOtp/VerifyOtpCommandHandler.cs
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using TeacherERP.Application.Features.Users.Commands.CreateStudent;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Users.Commands.VerifyOtp
{
    public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, Result<Guid>>
    {
        private readonly IMemoryCache _cache;
        private readonly IUserAccountService _userAccountService;
        private readonly IOtpService _otpService;

        public VerifyOtpCommandHandler(IMemoryCache cache, IUserAccountService userAccountService, IOtpService otpService)
        {
            _cache = cache;
            _userAccountService = userAccountService;
            _otpService = otpService;
        }

        public async Task<Result<Guid>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            // 1. جلب البيانات المعلقة من الـ Cache (قبل التحقق من الكود، حتى لا يُستهلك الكود بلا داعٍ)
            if (!_cache.TryGetValue($"Reg_{request.Email}", out CachedRegistrationData? cachedData) || cachedData == null)
            {
                return Result<Guid>.Failure(new Error("Otp.Expired", "OTP has expired or email is incorrect. Please register again."));
            }

            // 2. التحقق من الكود (يشمل حد المحاولات والقفل)
            var otpResult = await _otpService.VerifyAsync(OtpPurpose.Registration, request.Email, request.OtpCode, cancellationToken);
            if (otpResult.IsFailure)
            {
                return Result<Guid>.Failure(otpResult.Error);
            }

            // 3. الرمز صحيح! الآن نقوم بإنشاء كيان الطالب
            var student = ApplicationUser.CreateStudent(cachedData.FullName, cachedData.Email);

            // نجعل حسابه مفعلاً مسبقاً لأنه أثبت ملكية الإيميل للتو
            student.EmailConfirmed = true;

            // 4. حفظ المستخدم في قاعدة البيانات
            var creationResult = await _userAccountService.CreateUserAsync(student, cachedData.Password, RoleNames.Student, cancellationToken);

            if (creationResult.IsFailure)
            {
                return creationResult; // إرجاع الفشل إذا حدث خطأ أثناء الإنشاء (مثلاً باسورد ضعيف)
            }

            // 5. مسح البيانات من الـ Cache
            _cache.Remove($"Reg_{request.Email}");

            // 6. إرجاع الـ ID الخاص بالمستخدم الجديد بنجاح
            return Result<Guid>.Success(student.Id);
        }
    }
}