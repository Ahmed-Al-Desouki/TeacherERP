using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Profile.GetMyProfile
{
    public record GetMyProfileQuery : IRequest<Result<ProfileDto>>;
}