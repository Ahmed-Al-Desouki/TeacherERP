using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Students.GetWeeklySchedule;

public sealed class GetStudentWeeklyScheduleQueryHandler
    : IRequestHandler<GetStudentWeeklyScheduleQuery, Result<StudentWeeklyScheduleDto>>
{
    private readonly ICurrentUserService _currentUser;
    private readonly ICurrentTenantAccessor _currentTenant;
    private readonly IStudentWeeklyScheduleRepository _scheduleRepository;

    public GetStudentWeeklyScheduleQueryHandler(ICurrentUserService currentUser,
        ICurrentTenantAccessor currentTenant, IStudentWeeklyScheduleRepository scheduleRepository)
    {
        _currentUser = currentUser;
        _currentTenant = currentTenant;
        _scheduleRepository = scheduleRepository;
    }

    public async Task<Result<StudentWeeklyScheduleDto>> Handle(GetStudentWeeklyScheduleQuery request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is not { } studentId)
            return Result<StudentWeeklyScheduleDto>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));

        if (_currentTenant.Current is not { } tenant)
            return Result<StudentWeeklyScheduleDto>.Failure(TenantErrors.Required);

        if (tenant.Role != TenantRole.Student)
            return Result<StudentWeeklyScheduleDto>.Failure(TenantErrors.InsufficientRole);

        var schedule = await _scheduleRepository.GetForStudentAsync(
            tenant.TenantId, studentId, cancellationToken);
        return Result<StudentWeeklyScheduleDto>.Success(StudentWeeklyScheduleDto.From(schedule));
    }
}
