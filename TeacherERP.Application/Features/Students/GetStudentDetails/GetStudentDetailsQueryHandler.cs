using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Students.GetStudentDetails;

public sealed class GetStudentDetailsQueryHandler :
    IRequestHandler<GetStudentDetailsQuery, Result<StudentDetailsDto>>
{
    private readonly IStudentRepository _studentRepository;

    public GetStudentDetailsQueryHandler(IStudentRepository studentRepository)
        => _studentRepository = studentRepository;

    public async Task<Result<StudentDetailsDto>> Handle(
        GetStudentDetailsQuery request, CancellationToken cancellationToken)
    {
        var student = await _studentRepository.GetStudentDetailsAsync(request.StudentId, cancellationToken);
        return student is null
            ? Result<StudentDetailsDto>.Failure(StudentErrors.NotFound)
            : Result<StudentDetailsDto>.Success(StudentDetailsDto.From(student));
    }
}
