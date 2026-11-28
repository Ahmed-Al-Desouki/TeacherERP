// موجود (تعديل) — TeacherERP.Application/Features/Users/Commands/VerifyOtp/VerifyOtpCommandHandler.cs
using MediatR;
using TeacherERP.Application.Features.Users.Commands.CreateStudent;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Users.Commands.VerifyOtp
{
    public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, Result<Guid>>
    {
        private readonly ITransientAuthStateStore _authStateStore;
        private readonly IUserAccountService _userAccountService;
        private readonly IOtpService _otpService;

        public VerifyOtpCommandHandler(ITransientAuthStateStore authStateStore, IUserAccountService userAccountService, IOtpService otpService)
        {
            _authStateStore = authStateStore;
            _userAccountService = userAccountService;
            _otpService = otpService;
        }

        public async Task<Result<Guid>> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
        {
            // 1. جلب مسودة التسجيل قبل التحقق من الكود، حتى لا يُستهلك الكود بلا داعٍ
            var pendingData = await _authStateStore.GetPayloadAsync<PendingStudentRegistrationData>(OtpPurpose.Registration.ToString(), request.Email, cancellationToken);
            if (pendingData is null)
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
            var student = ApplicationUser.CreateStudent(pendingData.FullName, pendingData.Email);
            student.SetTimeZoneId(pendingData.TimeZoneId);
            student.SetProfileImageUrl(pendingData.ProfileImageUrl);

            // نجعل حسابه مفعلاً مسبقاً لأنه أثبت ملكية الإيميل للتو
            student.EmailConfirmed = true;

            // 4. حفظ المستخدم في قاعدة البيانات
            var creationResult = await _userAccountService.CreateUserWithPasswordHashAsync(student, pendingData.PasswordHash, RoleNames.Student, cancellationToken);

            if (creationResult.IsFailure)
            {
                return creationResult; // إرجاع الفشل إذا حدث خطأ أثناء الإنشاء (مثلاً باسورد ضعيف)
            }

            // 5. حذف مسودة التسجيل بعد الإنشاء الناجح
            await _authStateStore.RemovePayloadAsync(OtpPurpose.Registration.ToString(), request.Email, cancellationToken);

            // 6. إرجاع الـ ID الخاص بالمستخدم الجديد بنجاح
            return Result<Guid>.Success(student.Id);
        }
    }
}
