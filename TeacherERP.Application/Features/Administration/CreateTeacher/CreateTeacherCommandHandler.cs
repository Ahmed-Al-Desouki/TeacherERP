using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Administration.CreateTeacher
{
    public class CreateTeacherCommandHandler : IRequestHandler<CreateTeacherCommand, Result<CreatedTeacherDto>>
    {
        public const string DefaultTimeZoneId = "Africa/Cairo";

        private readonly ITeacherProvisioningService _teacherProvisioningService;

        public CreateTeacherCommandHandler(ITeacherProvisioningService teacherProvisioningService)
        {
            _teacherProvisioningService = teacherProvisioningService;
        }

        public Task<Result<CreatedTeacherDto>> Handle(CreateTeacherCommand request, CancellationToken cancellationToken)
        {
            var timeZoneId = string.IsNullOrWhiteSpace(request.TimeZoneId) ? DefaultTimeZoneId : request.TimeZoneId.Trim();

            return _teacherProvisioningService.ProvisionAsync(
                request.FullName.Trim(),
                request.Email.Trim().ToLowerInvariant(),
                timeZoneId,
                cancellationToken);
        }
    }
}
