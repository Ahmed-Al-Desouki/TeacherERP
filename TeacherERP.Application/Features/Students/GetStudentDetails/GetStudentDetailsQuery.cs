using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Students.GetStudentDetails;

public sealed record GetStudentDetailsQuery(Guid StudentId)
    : IRequest<Result<StudentDetailsDto>>;
