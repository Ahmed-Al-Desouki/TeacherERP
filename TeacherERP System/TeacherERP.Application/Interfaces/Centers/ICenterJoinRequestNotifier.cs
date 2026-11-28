namespace TeacherERP.Application.Interfaces
{
    public interface ICenterJoinRequestNotifier
    {
        Task<bool> NotifyOwnerAsync(Guid centerOwnerUserId, Guid teacherTenantId, string centerName, bool isReminder, CancellationToken cancellationToken);
    }
}
