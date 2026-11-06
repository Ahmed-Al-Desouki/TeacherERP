// جديد — TeacherERP.Application/Features/SessionContent/GetSessionAttachments/GetSessionAttachmentsValidator.cs
using FluentValidation;
using TeacherERP.Application.Common;

namespace TeacherERP.Application.Features.SessionContent.GetSessionAttachments
{
    public class GetSessionAttachmentsValidator : AbstractValidator<GetSessionAttachmentsQuery>
    {
        public GetSessionAttachmentsValidator()
        {
            Include(new PagedRequestRules<GetSessionAttachmentsQuery>());

            RuleFor(x => x.SessionId)
                .NotEmpty().WithMessage("Session id is required.");

            RuleFor(x => x.Kind)
                .IsInEnum().When(x => x.Kind.HasValue)
                .WithMessage("File kind is invalid.");
        }
    }
}