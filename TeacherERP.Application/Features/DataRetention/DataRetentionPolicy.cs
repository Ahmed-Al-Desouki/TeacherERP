namespace TeacherERP.Application.Features.DataRetention;

public static class DataRetentionPolicy
{
    public static readonly TimeSpan ReadNotificationRetention = TimeSpan.FromDays(180);
    public static readonly TimeSpan ClosedRequestRetention = TimeSpan.FromDays(365);
    public static readonly TimeSpan OrphanedFileGracePeriod = TimeSpan.FromDays(30);
    public const int DatabaseBatchSize = 250;
    public const int ExternalFileBatchSize = 100;
}
