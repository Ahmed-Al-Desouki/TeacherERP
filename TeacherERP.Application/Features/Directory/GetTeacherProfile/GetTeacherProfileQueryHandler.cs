// جديد — TeacherERP.Application/Features/Directory/GetTeacherProfile/GetTeacherProfileQueryHandler.cs
using MediatR;
using TeacherERP.Application.Features.Groups;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Directory.GetTeacherProfile
{
    public class GetTeacherProfileQueryHandler : IRequestHandler<GetTeacherProfileQuery, Result<TeacherProfileDto>>
    {
        private readonly IDirectoryRepository _directoryRepository;

        public GetTeacherProfileQueryHandler(IDirectoryRepository directoryRepository)
        {
            _directoryRepository = directoryRepository;
        }

        public async Task<Result<TeacherProfileDto>> Handle(GetTeacherProfileQuery request, CancellationToken cancellationToken)
        {
            var grade = string.IsNullOrWhiteSpace(request.GradeLevel) ? null : request.GradeLevel.Trim();

            var profile = await _directoryRepository.GetTeacherProfileAsync(request.TeacherId, grade, cancellationToken);
            if (profile is null)
            {
                return Result<TeacherProfileDto>.Failure(new Error("Directory.TeacherNotFound", "The teacher was not found."));
            }

            var groups = profile.Groups
                .Select(group =>
                {
                    var enrolled = profile.ActiveCountByGroup.GetValueOrDefault(group.Id);
                    int? available = group.Capacity is { } capacity ? Math.Max(0, capacity - enrolled) : null;
                    var dto = GroupDto.From(group);

                    return new DirectoryGroupDto(
                        group.Id, group.Name, group.GradeLevel, group.CenterId, group.Room, group.Capacity, available, dto.Slots);
                })
                .ToList();

            return Result<TeacherProfileDto>.Success(new TeacherProfileDto(
                profile.TeacherId,
                profile.Name,
                profile.Subject,
                profile.TimeZoneId,
                profile.Centers.Select(c => new DirectoryCenterInfoDto(c.Id, c.Name, c.Address)).ToList(),
                groups));
        }
    }
}