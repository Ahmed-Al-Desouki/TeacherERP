// موجود (تعديل) — TeacherERP.Application/Features/Students/GetStudents/GetStudentsQueryHandler.cs
using MediatR;
using TeacherERP.Application.Common;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Students.GetStudents
{
    public class GetStudentsQueryHandler : IRequestHandler<GetStudentsQuery, Result<PagedResult<StudentListDto>>>
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IStudentRepository _studentRepository;

        public GetStudentsQueryHandler(ICurrentTenantAccessor currentTenant, IStudentRepository studentRepository)
        {
            _currentTenant = currentTenant;
            _studentRepository = studentRepository;
        }

        public async Task<Result<PagedResult<StudentListDto>>> Handle(GetStudentsQuery request, CancellationToken cancellationToken)
        {
            if (_currentTenant.Current is null)
            {
                return Result<PagedResult<StudentListDto>>.Failure(TenantErrors.Required);
            }

            var page = await _studentRepository.GetStudentsPageAsync(
                request.Status, request.GroupId, PageRequest.From(request), cancellationToken);

            return Result<PagedResult<StudentListDto>>.Success(page.Map(StudentListDto.From));
        }
    }
}
