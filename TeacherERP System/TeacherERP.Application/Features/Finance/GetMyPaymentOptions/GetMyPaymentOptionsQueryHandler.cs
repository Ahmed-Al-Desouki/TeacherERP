using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Finance;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;
namespace TeacherERP.Application.Features.Finance.GetMyPaymentOptions;
public sealed class GetMyPaymentOptionsQueryHandler : IRequestHandler<GetMyPaymentOptionsQuery, Result<StudentPaymentDetails>>
{
    private readonly ICurrentUserService _user;
    private readonly ICurrentTenantAccessor _tenant;
    private readonly ITeacherPaymentService _payments;
    public GetMyPaymentOptionsQueryHandler(ICurrentUserService user, ICurrentTenantAccessor tenant, ITeacherPaymentService payments)
    { _user = user; _tenant = tenant; _payments = payments; }
    public Task<Result<StudentPaymentDetails>> Handle(GetMyPaymentOptionsQuery request, CancellationToken cancellationToken)
    {
        if (_user.UserId is not { } studentId) return Task.FromResult(Result<StudentPaymentDetails>.Failure(new Error("Auth.Unauthorized", "You must be logged in.")));
        if (_tenant.Current is not { } tenant) return Task.FromResult(Result<StudentPaymentDetails>.Failure(TenantErrors.Required));
        if (tenant.Role != TenantRole.Student) return Task.FromResult(Result<StudentPaymentDetails>.Failure(FinanceErrors.StudentOnly));
        return _payments.GetStudentPaymentDetailsAsync(tenant.TenantId, studentId, request.SessionId, cancellationToken);
    }
}
