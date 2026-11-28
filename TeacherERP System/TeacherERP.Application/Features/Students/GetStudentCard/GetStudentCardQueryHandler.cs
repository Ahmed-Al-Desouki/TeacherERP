using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Students.GetStudentCard;

public sealed class GetStudentCardQueryHandler : IRequestHandler<GetStudentCardQuery, Result<StudentCardDto>>
{
    private readonly ICurrentUserService _currentUser;
    private readonly ICurrentTenantAccessor _currentTenant;
    private readonly IUserRepository _users;

    public GetStudentCardQueryHandler(ICurrentUserService currentUser, ICurrentTenantAccessor currentTenant,
        IUserRepository users)
    {
        _currentUser = currentUser;
        _currentTenant = currentTenant;
        _users = users;
    }

    public async Task<Result<StudentCardDto>> Handle(GetStudentCardQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } studentId)
            return Result<StudentCardDto>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));

        if (_currentTenant.Current is not { } tenant)
            return Result<StudentCardDto>.Failure(TenantErrors.Required);

        if (tenant.Role != TenantRole.Student)
            return Result<StudentCardDto>.Failure(TenantErrors.InsufficientRole);

        var student = await _users.GetByIdAsync(studentId, cancellationToken);
        if (student is null)
            return Result<StudentCardDto>.Failure(new Error("Student.NotFound", "The student account was not found."));

        return Result<StudentCardDto>.Success(new StudentCardDto(student.Id, student.FullName, student.ProfileImageUrl, student.StaticQrCode));
    }
}
