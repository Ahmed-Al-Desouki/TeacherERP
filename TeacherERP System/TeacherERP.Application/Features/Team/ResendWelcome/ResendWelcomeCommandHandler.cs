// جديد — TeacherERP.Application/Features/Team/ResendWelcome/ResendWelcomeCommandHandler.cs
using System.Data;
using MediatR;
using Microsoft.Extensions.Logging;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Team.ResendWelcome
{
    public class ResendWelcomeCommandHandler : IRequestHandler<ResendWelcomeCommand, Result<bool>>
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly ITenantRepository _tenantRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICredentialService _credentialService;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ResendWelcomeCommandHandler> _logger;

        public ResendWelcomeCommandHandler(
            ICurrentTenantAccessor currentTenant,
            ITenantRepository tenantRepository,
            ITeamRepository teamRepository,
            IUserRepository userRepository,
            ICredentialService credentialService,
            IPasswordGenerator passwordGenerator,
            IEmailService emailService,
            IUnitOfWork unitOfWork,
            ILogger<ResendWelcomeCommandHandler> logger)
        {
            _currentTenant = currentTenant;
            _tenantRepository = tenantRepository;
            _teamRepository = teamRepository;
            _userRepository = userRepository;
            _credentialService = credentialService;
            _passwordGenerator = passwordGenerator;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(ResendWelcomeCommand request, CancellationToken cancellationToken)
        {
            if (_currentTenant.Current is not { } current)
            {
                return Result<bool>.Failure(TenantErrors.Required);
            }

            // عضوية مساعد عند مدرسي أنا فقط (غير الموجود وغير مساعدي نفس الرد)
            var member = await _teamRepository.GetTrackedAssistantAsync(request.UserId, cancellationToken);
            var user = member is null ? null : await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (member is null || user is null)
            {
                return Result<bool>.Failure(TeamErrors.NotFound);
            }

            // الحساب المفعّل ليه باسوردُه، ولو نسيه يستخدم "نسيت كلمة المرور"
            if (user.EmailConfirmed)
            {
                return Result<bool>.Failure(TeamErrors.AlreadyActivated);
            }

            var tenant = await _tenantRepository.GetByIdAsync(current.TenantId, cancellationToken);
            if (tenant is null)
            {
                return Result<bool>.Failure(TenantErrors.Forbidden);
            }

            var password = _passwordGenerator.Generate();
            var toEmail = user.Email!;
            var fullName = user.FullName;

            try
            {
                // تغيير الباسورد والإيميل وحدة واحدة: لو الإرسال فشل يرجع الباسورد القديم
                return await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var reset = await _credentialService.ResetPasswordAsync(user, password, cancellationToken);
                    if (reset.IsFailure)
                    {
                        return Result<bool>.Failure(reset.Error);
                    }

                    try
                    {
                        await _emailService.SendAssistantWelcomeAsync(toEmail, fullName, tenant.Name, password);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Welcome email failed while resending. The password change is rolled back.");
                        throw new WelcomeEmailFailedException();
                    }

                    return Result<bool>.Success(true);
                }, IsolationLevel.ReadCommitted, cancellationToken);
            }
            catch (WelcomeEmailFailedException)
            {
                return Result<bool>.Failure(TeamErrors.EmailSendFailed);
            }
        }

        // يُستخدم لإجبار الـ Transaction على التراجع عند فشل الإيميل
        private sealed class WelcomeEmailFailedException : Exception
        {
        }
    }
}