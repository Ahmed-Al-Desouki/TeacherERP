// جديد — TeacherERP.Application/Features/Team/CreateAssistant/CreateAssistantCommandHandler.cs
using System.Data;
using Hangfire;
using MediatR;
using Microsoft.Extensions.Logging;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Team.CreateAssistant
{
    public class CreateAssistantCommandHandler : IRequestHandler<CreateAssistantCommand, Result<AssistantDto>>
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly ITenantRepository _tenantRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IUserAccountService _userAccountService;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly IEmailService _emailService;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateAssistantCommandHandler> _logger;

        public CreateAssistantCommandHandler(
            ICurrentTenantAccessor currentTenant,
            ITenantRepository tenantRepository,
            ITeamRepository teamRepository,
            IUserAccountService userAccountService,
            IPasswordGenerator passwordGenerator,
            IEmailService emailService,
            IBackgroundJobClient backgroundJobClient,
            IUnitOfWork unitOfWork,
            ILogger<CreateAssistantCommandHandler> logger)
        {
            _currentTenant = currentTenant;
            _tenantRepository = tenantRepository;
            _teamRepository = teamRepository;
            _userAccountService = userAccountService;
            _passwordGenerator = passwordGenerator;
            _emailService = emailService;
            _backgroundJobClient = backgroundJobClient;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<AssistantDto>> Handle(CreateAssistantCommand request, CancellationToken cancellationToken)
        {
            // السنتر الحالي ثبّته الـ Policy (TenantOwner) بعد التحقق من العضوية والدور
            if (_currentTenant.Current is not { } current)
            {
                return Result<AssistantDto>.Failure(TenantErrors.Required);
            }

            var tenant = await _tenantRepository.GetByIdAsync(current.TenantId, cancellationToken);
            if (tenant is null)
            {
                return Result<AssistantDto>.Failure(TenantErrors.Forbidden);
            }

            var email = request.Email.Trim();
            var fullName = request.FullName.Trim();

            var existing = (await _userAccountService.FindByEmailAsync(email, cancellationToken)).Value;

            return existing is null
                ? await CreateNewAccountAsync(current.TenantId, tenant.Name, fullName, email, cancellationToken)
                : await AddExistingAssistantAsync(current.TenantId, tenant.Name, existing, cancellationToken);
        }

        // حساب جديد: باسورد مؤقت يُرسل للمساعد نفسه داخل الـ Transaction (فشل الإيميل = لا حساب)
        private async Task<Result<AssistantDto>> CreateNewAccountAsync(
            Guid tenantId, string teacherName, string fullName, string email, CancellationToken cancellationToken)
        {
            if (await _teamRepository.CountAssistantsAsync(cancellationToken) >= TeamPolicy.MaxAssistants)
            {
                return Result<AssistantDto>.Failure(TeamErrors.LimitReached);
            }

            var password = _passwordGenerator.Generate();

            try
            {
                return await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    var user = ApplicationUser.CreateAssistant(fullName, email);

                    var created = await _userAccountService.CreateUserAsync(user, password, RoleNames.Assistant, cancellationToken);
                    if (created.IsFailure)
                    {
                        return Result<AssistantDto>.Failure(created.Error);
                    }

                    _teamRepository.AddMember(TenantMember.Create(tenantId, user.Id, TenantRole.Assistant));
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    // آخر خطوة قبل الـ Commit: لو فشل الإرسال تتراجع كل التغييرات
                    try
                    {
                        await _emailService.SendAssistantWelcomeAsync(email, fullName, teacherName, password);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Welcome email failed for a new assistant. The account creation is rolled back.");
                        throw new WelcomeEmailFailedException();
                    }

                    return Result<AssistantDto>.Success(
                        new AssistantDto(user.Id, user.FullName, email, IsActivated: false, IsExistingAccount: false));
                }, IsolationLevel.ReadCommitted, cancellationToken);
            }
            catch (WelcomeEmailFailedException)
            {
                return Result<AssistantDto>.Failure(TeamErrors.EmailSendFailed);
            }
        }

        // مساعد موجود (عند مدرس آخر): نضيف عضويته فقط بدون باسورد، ونبلّغه بإيميل
        private async Task<Result<AssistantDto>> AddExistingAssistantAsync(
            Guid tenantId, string teacherName, ApplicationUser existing, CancellationToken cancellationToken)
        {
            var roles = (await _userAccountService.GetRolesAsync(existing, cancellationToken)).Value;

            var isEligible = roles is not null
                             && roles.Contains(RoleNames.Assistant, StringComparer.OrdinalIgnoreCase)
                             && !roles.Contains(RoleNames.SuperAdmin, StringComparer.OrdinalIgnoreCase)
                             && !roles.Contains(RoleNames.Student, StringComparer.OrdinalIgnoreCase)
                             && existing.Status == StudentStatus.Active;

            if (!isEligible)
            {
                return Result<AssistantDto>.Failure(TeamErrors.EmailNotAvailable);
            }

            if (await _teamRepository.IsMemberAsync(existing.Id, cancellationToken))
            {
                return Result<AssistantDto>.Failure(TeamErrors.AlreadyMember);
            }

            if (await _teamRepository.CountAssistantsAsync(cancellationToken) >= TeamPolicy.MaxAssistants)
            {
                return Result<AssistantDto>.Failure(TeamErrors.LimitReached);
            }

            _teamRepository.AddMember(TenantMember.Create(tenantId, existing.Id, TenantRole.Assistant));
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var toEmail = existing.Email!;
            var assistantName = existing.FullName;
            _backgroundJobClient.Enqueue<IEmailService>(emailService =>
                emailService.SendAddedToTeamAlertAsync(toEmail, assistantName, teacherName));

            return Result<AssistantDto>.Success(
                new AssistantDto(existing.Id, existing.FullName, toEmail, existing.EmailConfirmed, IsExistingAccount: true));
        }

        // يُستخدم لإجبار الـ Transaction على التراجع عند فشل الإيميل (النتيجة الفاشلة لوحدها كانت ستُعمل Commit)
        private sealed class WelcomeEmailFailedException : Exception
        {
        }
    }
}