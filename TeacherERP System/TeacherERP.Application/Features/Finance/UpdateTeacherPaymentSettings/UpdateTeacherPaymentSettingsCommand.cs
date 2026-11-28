using FluentValidation;
using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;
namespace TeacherERP.Application.Features.Finance.UpdateTeacherPaymentSettings;
public sealed record UpdateTeacherPaymentSettingsCommand(string InstaPayLink, Stream? QrImage, string? FileName) : IRequest<Result<TeacherPaymentSettingsDto>>;
public sealed class UpdateTeacherPaymentSettingsValidator : AbstractValidator<UpdateTeacherPaymentSettingsCommand>
{
    public UpdateTeacherPaymentSettingsValidator()
    { RuleFor(x => x.InstaPayLink).NotEmpty().MaximumLength(2048); RuleFor(x => x.FileName).MaximumLength(256); }
}
