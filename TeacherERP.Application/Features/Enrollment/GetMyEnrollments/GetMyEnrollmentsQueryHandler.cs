// جديد — TeacherERP.Application/Features/Enrollment/GetMyEnrollments/GetMyEnrollmentsQueryHandler.cs
using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Enrollment.GetMyEnrollments
{
    public class GetMyEnrollmentsQueryHandler : IRequestHandler<GetMyEnrollmentsQuery, Result<MyEnrollmentsDto>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IEnrollmentRepository _repository;

        public GetMyEnrollmentsQueryHandler(ICurrentUserService currentUser, IEnrollmentRepository repository)
        {
            _currentUser = currentUser;
            _repository = repository;
        }

        public async Task<Result<MyEnrollmentsDto>> Handle(GetMyEnrollmentsQuery request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } studentId)
            {
                return Result<MyEnrollmentsDto>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            var enrollments = await _repository.GetMyEnrollmentsAsync(studentId, cancellationToken);
            var requests = await _repository.GetMyRequestsAsync(studentId, cancellationToken);

            return Result<MyEnrollmentsDto>.Success(new MyEnrollmentsDto(
                enrollments.Select(MyEnrollmentDto.From).ToList(),
                requests.Select(EnrollmentRequestDto.From).ToList()));
        }
    }
}