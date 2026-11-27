namespace TeacherERP.Application.Features.TeacherRequests;

public sealed class PendingTeacherRequestData
{
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Subject { get; init; } = string.Empty;
    public string? Note { get; init; }
    public string TimeZoneId { get; init; } = string.Empty;
}
