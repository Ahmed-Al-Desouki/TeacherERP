using MediatR;
using TeacherERP.Domain.Shared;
using TeacherERP.Application.Features.Enrollment.RequestGroup;

namespace TeacherERP.Application.Features.Enrollment.RequestGroupByCode;

public sealed record RequestGroupByCodeCommand(string Code) : IRequest<Result<EnrollmentRequestDto>>;
