// جديد — TeacherERP.Application/Features/Students/GetStudents/GetStudentsQueryHandler.cs
using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Students.GetStudents
{
    public class GetStudentsQueryHandler : IRequestHandler<GetStudentsQuery, Result<List<StudentListDto>>>
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IStudentRepository _studentRepository;

        public GetStudentsQueryHandler(ICurrentTenantAccessor currentTenant, IStudentRepository studentRepository)
        {
            _currentTenant = currentTenant;
            _studentRepository = studentRepository;
        }

        public async Task<Result<List<StudentListDto>>> Handle(GetStudentsQuery request, CancellationToken cancellationToken)
        {
            if (_currentTenant.Current is null)
            {
                return Result<List<StudentListDto>>.Failure(TenantErrors.Required);
            }

            var items = await _studentRepository.GetStudentsAsync(request.Status, request.GroupId, cancellationToken);

            return Result<List<StudentListDto>>.Success(items.Select(StudentListDto.From).ToList());
        }
    }
}