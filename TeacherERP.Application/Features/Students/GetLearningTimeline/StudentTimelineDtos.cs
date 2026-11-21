using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Features.Students.GetLearningTimeline;

public sealed record StudentTimelineAttachmentDto(
    Guid Id,
    SessionAttachmentKind Kind,
    string Url,
    string FileName,
    string ContentType,
    long SizeBytes,
    string? Caption,
    DateTime CreatedAt)
{
    public static StudentTimelineAttachmentDto From(SessionAttachmentItem item) =>
        new(item.Id, item.Kind, item.Url, item.FileName, item.ContentType,
            item.SizeBytes, item.Caption, item.CreatedAt);
}

public sealed record StudentTimelineSessionDto(
    Guid SessionId,
    Guid GroupId,
    string GroupName,
    DateTime StartTime,
    DateTime EndTime,
    bool IsCancelled,
    string? CancelReason,
    AttendanceStatus? AttendanceStatus,
    IReadOnlyList<StudentTimelineAttachmentDto> Attachments)
{
    public static StudentTimelineSessionDto From(StudentTimelineSessionItem session,
        IReadOnlyList<StudentTimelineAttachmentDto> attachments) =>
        new(session.SessionId, session.GroupId, session.GroupName, session.StartTime,
            session.EndTime, session.IsCancelled, session.CancelReason,
            session.AttendanceStatus, attachments);
}
