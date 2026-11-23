using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Students.GetStudentCard;

public sealed record GetStudentCardQuery : IRequest<Result<StudentCardDto>>;
