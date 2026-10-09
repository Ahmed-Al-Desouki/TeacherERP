using MediatR;
using TeacherERP.Application.Features.Profile;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;
using TeacherERP.Domain.ValueObjects;

namespace TeacherERP.Application.Features.Profile.CompleteProfile
{
    public class CompleteProfileCommandHandler : IRequestHandler<CompleteProfileCommand, Result<ProfileDto>>
    {
        private readonly ICurrentUserService _currentUser;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CompleteProfileCommandHandler(
            ICurrentUserService currentUser,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _currentUser = currentUser;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ProfileDto>> Handle(CompleteProfileCommand request, CancellationToken cancellationToken)
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

            // 1. توحيد صيغة الأرقام (تم التحقق منها في الـ Validator)
            if (!EgyptianPhoneNumber.TryNormalize(request.PhoneNumber, out var phoneNumber)
                || !EgyptianPhoneNumber.TryNormalize(request.GuardianPhoneNumber, out var guardianPhoneNumber))
            {
                return Result<ProfileDto>.Failure(new Error("Profile.InvalidPhoneNumber", "A valid Egyptian mobile number is required."));
            }

            // 2. موبايل الطالب فريد في النظام
            if (await _userRepository.PhoneNumberExistsAsync(phoneNumber, userId, cancellationToken))
            {
                return Result<ProfileDto>.Failure(new Error("Profile.PhoneNumberTaken", "This phone number is already used by another account."));
            }

            // 3. تطبيق قواعد الـ Domain (مثل: موبايل ولي الأمر يختلف عن موبايل الطالب)
            var guardian = GuardianInfo.Create(request.GuardianName, guardianPhoneNumber, request.GuardianRelation);
            var result = user.CompleteProfile(phoneNumber, guardian, request.Gender, request.DateOfBirth);
            if (result.IsFailure)
            {
                return Result<ProfileDto>.Failure(result.Error);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<ProfileDto>.Success(ProfileDto.From(user));
        }
    }
}