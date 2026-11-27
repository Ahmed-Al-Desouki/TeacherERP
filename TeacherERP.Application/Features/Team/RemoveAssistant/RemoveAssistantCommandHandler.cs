// جديد — TeacherERP.Application/Features/Team/RemoveAssistant/RemoveAssistantCommandHandler.cs
using System.Data;
using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Team.RemoveAssistant
{
    public class RemoveAssistantCommandHandler : IRequestHandler<RemoveAssistantCommand, Result<bool>>
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly ICurrentUserService _currentUser;
        private readonly ITeamRepository _teamRepository;
        private readonly IAuditRepository _auditRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserAccountService _userAccountService;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveAssistantCommandHandler(
            ICurrentTenantAccessor currentTenant,
            ICurrentUserService currentUser,
            ITeamRepository teamRepository,
            IAuditRepository auditRepository,
            IUserRepository userRepository,
            IUserAccountService userAccountService,
            IUnitOfWork unitOfWork)
        {
            _currentTenant = currentTenant;
            _currentUser = currentUser;
            _teamRepository = teamRepository;
            _auditRepository = auditRepository;
            _userRepository = userRepository;
            _userAccountService = userAccountService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(RemoveAssistantCommand request, CancellationToken cancellationToken)
        {
            if (_currentTenant.Current is not { } current)
            {
                return Result<bool>.Failure(TenantErrors.Required);
            }

            if (_currentUser.UserId is not { } actorId)
            {
                return Result<bool>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            var member = await _teamRepository.GetTrackedAssistantAsync(request.UserId, cancellationToken);
            if (member is null)
            {
                return Result<bool>.Failure(TeamErrors.NotFound);
            }

            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // الوصول يتقطع فوراً: TenantHandler يتحقق من العضوية في كل طلب. الحضور والموافقات القديمة تبقى (تشاور على الحساب)
                _teamRepository.RemoveMember(member);
                _auditRepository.Add(AuditLog.Create(current.TenantId, actorId, AuditAction.AssistantRemoved, member.UserId, null));
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // مساعد لسه ما فعّلش ومدرسه الوحيد = دعوة ملغاة: نمسح الحساب حتى لا تبقى حسابات يتيمة
                // (المفعّل يبقى حسابه ودوره، فيمكن إعادة إضافته أو يعمل عند مدرس آخر)
                if (user is { EmailConfirmed: false }
                    && !await _teamRepository.HasOtherMembershipsAsync(user.Id, current.TenantId, cancellationToken))
                {
                    var deleted = await _userAccountService.DeleteAsync(user, cancellationToken);
                    if (deleted.IsFailure)
                    {
                        // استثناء عمداً لتتراجع الـ Transaction (النتيجة الفاشلة كانت ستُعمل Commit للعضوية المحذوفة)
                        throw new InvalidOperationException($"Failed to delete the pending assistant account: {deleted.Error.Message}");
                    }
                }

                return Result<bool>.Success(true);
            }, IsolationLevel.ReadCommitted, cancellationToken);
        }
    }
}
