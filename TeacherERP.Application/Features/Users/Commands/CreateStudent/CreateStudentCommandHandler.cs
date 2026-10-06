using MediatR;
using Hangfire;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;
using TeacherERP.Application.Interfaces;

namespace TeacherERP.Application.Features.Users.Commands.CreateStudent
{
    // مسؤول حصرياً عن الـ Business Logic
    public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, Result<Guid>>
    {
        private readonly IIdentityService _identityService;
        private readonly IOtpService _otpService;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public CreateStudentCommandHandler(
            IIdentityService identityService,
            IOtpService otpService,
            IBackgroundJobClient backgroundJobClient)
        {
            _identityService = identityService;
            _otpService = otpService;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task<Result<Guid>> Handle(CreateStudentCommand request, CancellationToken cancellationToken)
        {
            // 1. التحقق من عدم وجود الإيميل مسبقاً (قاعدة عمل / Business Rule)
            var emailExistsResult = await _identityService.CheckEmailExistsAsync(request.Email, cancellationToken);
            if (emailExistsResult.Value)
            {
                return Result<Guid>.Failure(new Error("User.EmailExists", "This email is already registered."));
            }

            // 2. إنشاء الكيان عبر دالة المصنع لضمان صحة الحالة[cite: 3]
            var student = ApplicationUser.CreateStudent(request.FullName, request.Email, request.PhoneNumber);

            // 3. إنشاء المستخدم وإعطائه الصلاحية عبر Identity Service المعزولة
            var creationResult = await _identityService.CreateUserAsync(student, request.Password, "Student", cancellationToken);
            if (creationResult.IsFailure)
            {
                return creationResult; // إرجاع الفشل مباشرة
            }

            // 4. توليد الـ OTP
            var otpCode = await _otpService.GenerateAndSaveOtpAsync(student.Id, cancellationToken);

            // 5. جدولة الإيميل في الخلفية (Fire-and-Forget) لضمان سرعة الاستجابة
            _backgroundJobClient.Enqueue<IEmailService>(emailService =>
                emailService.SendOtpEmailAsync(student.Email, otpCode));

            // 6. إرجاع المعرف (Guid) الخاص بالطالب كنجاح
            return Result<Guid>.Success(student.Id);
        }
    }
}