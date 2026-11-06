// جديد — TeacherERP.Application/Features/SessionContent/UploadSessionAttachment/UploadSessionAttachmentValidator.cs
using FluentValidation;

namespace TeacherERP.Application.Features.SessionContent.UploadSessionAttachment
{
    // حجم ونوع الملف يُفحصان في الـ Handler (بأخطاء موحدة)، وهنا الحقول النصية فقط
    public class UploadSessionAttachmentValidator : AbstractValidator<UploadSessionAttachmentCommand>
    {
        public UploadSessionAttachmentValidator()
        {
            RuleFor(x => x.SessionId)
                .NotEmpty().WithMessage("Session id is required.");

            RuleFor(x => x.Kind)
                .IsInEnum().WithMessage("File kind is invalid.");

            RuleFor(x => x.Caption)
                .MaximumLength(300).WithMessage("Caption must not exceed 300 characters.");

            RuleFor(x => x.FileName)
                .NotEmpty().WithMessage("File name is required.")
                .MaximumLength(255).WithMessage("File name must not exceed 255 characters.");

            RuleFor(x => x.Content)
                .NotNull().WithMessage("File content is required.");
        }
    }
}