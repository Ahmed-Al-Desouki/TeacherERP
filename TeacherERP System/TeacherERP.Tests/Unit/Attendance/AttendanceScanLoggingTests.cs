using Microsoft.Extensions.Logging;
using TeacherERP.Application.Attendance;
using TeacherERP.Application.Features.Attendance;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Tests.Unit.Attendance;

public sealed class AttendanceScanLoggingTests
{
    [Fact]
    public void Rejected_LogsCodeAndTraceIdentifiersWithoutSensitiveValues()
    {
        var logger = new CapturingLogger();
        var tenantId = Guid.NewGuid();
        var actorId = Guid.NewGuid();
        var groupId = Guid.NewGuid();

        AttendanceScanLogging.Rejected(
            logger, AttendanceErrors.MockLocation, tenantId, actorId, actorId, groupId,
            sessionId: null, method: ScanMethod.SelfScan);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Warning, entry.Level);
        Assert.Equal("Attendance.MockLocation", entry.Fields["ErrorCode"]);
        Assert.Equal("Rejected", entry.Fields["Outcome"]);
        Assert.Equal(tenantId, entry.Fields["TenantId"]);
        Assert.Equal(actorId, entry.Fields["ActorUserId"]);
        Assert.Equal(groupId, entry.Fields["GroupId"]);
        AssertSafeFields(entry);
    }

    [Fact]
    public void Completed_LogsOutcomeAndIdentifiersWithoutLocation()
    {
        var logger = new CapturingLogger();
        var tenantId = Guid.NewGuid();
        var studentId = Guid.NewGuid();
        var groupId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var request = new AttendanceRequest(
            tenantId, studentId, ScanMethod.SelfScan, 30.0561, 31.3301,
            RecordedByUserId: null, PreferredGroupId: groupId, EnforceGeofence: true,
            RegisterImmediately: true, RestrictToGroupId: null, AccuracyMeters: 2);

        AttendanceScanLogging.Completed(
            logger, request, groupId, sessionId, AttendanceOutcome.AlreadyRecorded);

        var entry = Assert.Single(logger.Entries);
        Assert.Equal(LogLevel.Information, entry.Level);
        Assert.Equal(AttendanceOutcome.AlreadyRecorded.ToString(), entry.Fields["Outcome"]?.ToString());
        Assert.Equal(tenantId, entry.Fields["TenantId"]);
        Assert.Equal(studentId, entry.Fields["ActorUserId"]);
        Assert.Equal(studentId, entry.Fields["StudentId"]);
        Assert.Equal(sessionId, entry.Fields["SessionId"]);
        AssertSafeFields(entry);
    }

    private static void AssertSafeFields(CapturedLog entry)
    {
        Assert.DoesNotContain(entry.Fields.Keys, key =>
            key.Equals("Code", StringComparison.OrdinalIgnoreCase)
            || key.Contains("Qr", StringComparison.OrdinalIgnoreCase)
            || key.Contains("Latitude", StringComparison.OrdinalIgnoreCase)
            || key.Contains("Longitude", StringComparison.OrdinalIgnoreCase)
            || key.Contains("Accuracy", StringComparison.OrdinalIgnoreCase)
            || key.Contains("Name", StringComparison.OrdinalIgnoreCase)
            || key.Contains("Email", StringComparison.OrdinalIgnoreCase));
    }

    private sealed record CapturedLog(LogLevel Level, string Message, IReadOnlyDictionary<string, object?> Fields);

    private sealed class CapturingLogger : ILogger
    {
        public List<CapturedLog> Entries { get; } = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
            Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var fields = state is IEnumerable<KeyValuePair<string, object?>> values
                ? values.ToDictionary(pair => pair.Key, pair => pair.Value)
                : new Dictionary<string, object?>();
            Entries.Add(new CapturedLog(logLevel, formatter(state, exception), fields));
        }
    }
}
