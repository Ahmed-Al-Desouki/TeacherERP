using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;
namespace TeacherERP.Application.Features.Finance.UpdateTeacherPaymentSettings;
public sealed class UpdateTeacherPaymentSettingsCommandHandler : IRequestHandler<UpdateTeacherPaymentSettingsCommand, Result<TeacherPaymentSettingsDto>>
{
    private readonly ICurrentUserService _user; private readonly ICurrentTenantAccessor _tenant; private readonly ITeacherPaymentService _payments;
    public UpdateTeacherPaymentSettingsCommandHandler(ICurrentUserService user, ICurrentTenantAccessor tenant, ITeacherPaymentService payments) { _user = user; _tenant = tenant; _payments = payments; }
    public Task<Result<TeacherPaymentSettingsDto>> Handle(UpdateTeacherPaymentSettingsCommand request, CancellationToken cancellationToken)
    {
        if (_user.UserId is not { } ownerId) return Task.FromResult(Result<TeacherPaymentSettingsDto>.Failure(new Error("Auth.Unauthorized", "You must be logged in.")));
        if (_tenant.Current is not { } tenant) return Task.FromResult(Result<TeacherPaymentSettingsDto>.Failure(TenantErrors.Required));
        return _payments.UpdateSettingsAsync(tenant.TenantId, ownerId, request.InstaPayLink, request.QrImage, request.FileName, cancellationToken);
    }
}
