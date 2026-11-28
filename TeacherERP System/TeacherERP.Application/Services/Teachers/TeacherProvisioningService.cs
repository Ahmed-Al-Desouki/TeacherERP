using System.Data;
using Microsoft.Extensions.Logging;
using TeacherERP.Application.Features.Administration;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Teachers
{
    public class TeacherProvisioningService : ITeacherProvisioningService
    {
        private readonly IUserAccountService _userAccountService;
        private readonly ITenantRepository _tenantRepository;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TeacherProvisioningService> _logger;

        public TeacherProvisioningService(
            IUserAccountService userAccountService,
            ITenantRepository tenantRepository,
            IPasswordGenerator passwordGenerator,
            IEmailService emailService,
            IUnitOfWork unitOfWork,
            ILogger<TeacherProvisioningService> logger)
        {
            _userAccountService = userAccountService;
            _tenantRepository = tenantRepository;
            _passwordGenerator = passwordGenerator;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // الباسورد المؤقت يُرسل للمدرس نفسه داخل الـ Transaction (فشل الإيميل = لا حساب ولا مساحة)
        public async Task<Result<CreatedTeacherDto>> ProvisionAsync(
            string fullName, string email, string timeZoneId, CancellationToken cancellationToken)
        {
            var existing = (await _userAccountService.FindByEmailAsync(email, cancellationToken)).Value;
            if (existing is not null)
            {
                return Result<CreatedTeacherDto>.Failure(AdministrationErrors.EmailInUse);
            }

            var password = _passwordGenerator.Generate();

            try
            {
                return await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var user = ApplicationUser.CreateTeacher(fullName, email);
                    user.SetTimeZoneId(timeZoneId);

                    var created = await _userAccountService.CreateUserAsync(user, password, RoleNames.SuperAdmin, cancellationToken);
                    if (created.IsFailure)
                    {
                        return Result<CreatedTeacherDto>.Failure(created.Error);
                    }

                    var tenant = Tenant.Create(user.FullName, user.Id, timeZoneId);
                    _tenantRepository.AddTeacherSpace(
                        tenant,
                        TenantMember.Create(tenant.Id, user.Id, TenantRole.Owner),
                        TenantSettings.Create(tenant.Id));
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    // آخر خطوة قبل الـ Commit: لو فشل الإرسال تتراجع كل التغييرات
                    try
                    {
                        await _emailService.SendTeacherWelcomeAsync(email, fullName, password);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Welcome email failed for a new teacher. The account creation is rolled back.");
                        throw new WelcomeEmailFailedException();
                    }

                    return Result<CreatedTeacherDto>.Success(
                        new CreatedTeacherDto(user.Id, tenant.Id, user.FullName, email, IsActivated: false));
                }, IsolationLevel.ReadCommitted, cancellationToken);
            }
            catch (WelcomeEmailFailedException)
            {
                return Result<CreatedTeacherDto>.Failure(AdministrationErrors.EmailSendFailed);
            }
        }

        // يُستخدم لإجبار الـ Transaction على التراجع عند فشل الإيميل
        private sealed class WelcomeEmailFailedException : Exception
        {
        }
    }
}
