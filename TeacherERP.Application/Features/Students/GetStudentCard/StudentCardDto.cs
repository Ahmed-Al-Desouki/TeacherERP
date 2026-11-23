namespace TeacherERP.Application.Features.Students.GetStudentCard;

public sealed record StudentCardDto(Guid StudentId, string FullName, string QrCode);
