using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Users.Commands.PromoteToAdmin
{
    public class PromoteToAdminCommandHandler : IRequestHandler<PromoteToAdminCommand, Result<bool>>
    {
        private readonly IIdentityService _identityService;

        public PromoteToAdminCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Result<bool>> Handle(PromoteToAdminCommand request, CancellationToken cancellationToken)
        {
            return await _identityService.PromoteToAdminAsync(request.UserId, cancellationToken);
        }
    }
}