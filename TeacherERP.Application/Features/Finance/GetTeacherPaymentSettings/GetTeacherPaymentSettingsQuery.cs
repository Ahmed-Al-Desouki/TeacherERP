using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;
namespace TeacherERP.Application.Features.Finance.GetTeacherPaymentSettings;
public sealed record GetTeacherPaymentSettingsQuery : IRequest<Result<TeacherPaymentSettingsDto?>>;
