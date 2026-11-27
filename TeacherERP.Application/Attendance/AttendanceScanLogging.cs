using System.Diagnostics;
using Microsoft.Extensions.Logging;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Attendance;

/// <summary>Structured attendance events. Never pass QR codes, scan codes, coordinates, or user names here.</summary>
public static class AttendanceScanLogging
{
    public static void Rejected(
        ILogger logger,
        Error error,
        Guid? tenantId,
        Guid? actorUserId,
        Guid? studentId,
        Guid? groupId,
        Guid? sessionId,
        ScanMethod method)
    {
        logger.LogWarning(
            "Attendance scan rejected. TraceId {TraceId} TenantId {TenantId} ActorUserId {ActorUserId} StudentId {StudentId} GroupId {GroupId} SessionId {SessionId} ScanMethod {ScanMethod} Outcome {Outcome} ErrorCode {ErrorCode}",
            Activity.Current?.TraceId.ToString(), tenantId, actorUserId, studentId, groupId, sessionId,
            method, "Rejected", error.Code);
    }

    public static void Completed(
        ILogger logger,
        AttendanceRequest request,
        Guid groupId,
        Guid sessionId,
        AttendanceOutcome outcome)
    {
        logger.LogInformation(
            "Attendance scan completed. TraceId {TraceId} TenantId {TenantId} ActorUserId {ActorUserId} StudentId {StudentId} GroupId {GroupId} SessionId {SessionId} ScanMethod {ScanMethod} Outcome {Outcome} ErrorCode {ErrorCode}",
            Activity.Current?.TraceId.ToString(), request.TenantId,
            request.RecordedByUserId ?? request.StudentId, request.StudentId, groupId, sessionId,
            request.Method, outcome, null);
    }
}
