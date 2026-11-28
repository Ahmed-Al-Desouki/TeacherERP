using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Profile.GetMyProfile
{
    public class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, Result<ProfileDto>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IUserRepository _userRepository;

        public GetMyProfileQueryHandler(ICurrentUserService currentUser, IUserRepository userRepository)
        {
            _currentUser = currentUser;
            _userRepository = userRepository;
        }

        public async Task<Result<ProfileDto>> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId is not { } userId)
            {
                return Result<ProfileDto>.Failure(new Error("Auth.Unauthorized", "You must be logged in."));
            }

            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user is null)
            {
                return Result<ProfileDto>.Failure(new Error("User.NotFound", "User not found."));
            }

            return Result<ProfileDto>.Success(ProfileDto.From(user));
        }
    }
}