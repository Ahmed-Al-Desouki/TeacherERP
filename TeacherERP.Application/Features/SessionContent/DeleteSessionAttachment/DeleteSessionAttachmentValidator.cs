// جديد — TeacherERP.Application/Features/SessionContent/DeleteSessionAttachment/DeleteSessionAttachmentValidator.cs
using FluentValidation;

namespace TeacherERP.Application.Features.SessionContent.DeleteSessionAttachment
{
    public class DeleteSessionAttachmentValidator : AbstractValidator<DeleteSessionAttachmentCommand>
    {
        public DeleteSessionAttachmentValidator()
        {
            RuleFor(x => x.AttachmentId)
                .NotEmpty().WithMessage("Attachment id is required.");
        }
    }
}