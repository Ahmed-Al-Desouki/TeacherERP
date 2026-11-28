using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherERP.Application.Features.Profile.CompleteProfile;
using TeacherERP.Application.Features.Profile.GetMyProfile;
using TeacherERP.Application.Interfaces;
using TeacherERP.API.Files;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.ValueObjects;

namespace TeacherERP.API.Controllers
{
    // بروفايل المستخدم الحالي. المسار المطلق (~/) يعطي: /api/Users/me/profile
    [Authorize]
    public class ProfileController : BaseApiController
    {
        private readonly IMediator _mediator;
        private readonly IFileStorage _fileStorage;

        public ProfileController(IMediator mediator, IFileStorage fileStorage)
        {
            _mediator = mediator;
            _fileStorage = fileStorage;
        }

        [HttpGet("~/api/Users/me/profile")]
        public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetMyProfileQuery(), cancellationToken);
            return ToResponse(result);
        }

        [HttpPut("~/api/Users/me/profile")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CompleteProfile([FromForm] CompleteProfileForm form,
            CancellationToken cancellationToken)
        {
            string? profileImageUrl = null;
            if (form.ProfileImage is not null)
            {
                var stored = await ProfileImageUploader.UploadAsync(form.ProfileImage, _fileStorage, cancellationToken);
                if (stored.IsFailure) return ToErrorResponse(stored.Error.Code, stored.Error.Message);
                profileImageUrl = stored.Value!.Url;
            }

            var command = new CompleteProfileCommand(form.PhoneNumber, form.GuardianName,
                form.GuardianPhoneNumber, form.GuardianRelation, form.Gender, form.DateOfBirth, profileImageUrl);
            var result = await _mediator.Send(command, cancellationToken);
            return ToResponse(result);
        }
    }

    public sealed class CompleteProfileForm
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public string GuardianName { get; set; } = string.Empty;
        public string GuardianPhoneNumber { get; set; } = string.Empty;
        public GuardianRelation GuardianRelation { get; set; }
        public Gender? Gender { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public IFormFile? ProfileImage { get; set; }
    }
}
