namespace TeacherERP.Application.Features.Users.Commands.CreateStudent;

public sealed class PendingStudentRegistrationData
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string TimeZoneId { get; set; } = "UTC";
}
