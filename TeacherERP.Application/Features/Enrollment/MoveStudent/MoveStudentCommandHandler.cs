using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Enrollment.MoveStudent;

public sealed class MoveStudentCommandHandler : IRequestHandler<MoveStudentCommand, Result<bool>>
{
    private readonly ICurrentUserService _currentUser;
    private readonly ICurrentTenantAccessor _currentTenant;
    private readonly IEnrollmentService _enrollmentService;

    public MoveStudentCommandHandler(
        ICurrentUserService currentUser,
        ICurrentTenantAccessor currentTenant,
        IEnrollmentService enrollmentService)
    {
        _currentUser = currentUser;
        _currentTenant = currentTenant;
        _enrollmentService = enrollmentService;
    }

    public Task<Result<bool>> Handle(MoveStudentCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } userId)
            return Task.FromResult(Result<bool>.Failure(new Error("Auth.Unauthorized", "You must be logged in.")));

        if (_currentTenant.Current is not { } tenant)
            return Task.FromResult(Result<bool>.Failure(TenantErrors.Required));

        return _enrollmentService.MoveStudentAsync(
            tenant.TenantId, request.StudentId, request.TargetGroupId, userId, cancellationToken);
    }
}
