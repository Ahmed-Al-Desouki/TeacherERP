namespace TeacherERP.Application.Interfaces;

public interface IGroupJoinCodeService
{
    string CreateDynamicCode(Guid groupId, string secret, DateTime atUtc);
    bool TryGetDynamicGroupId(string code, out Guid groupId);
    bool IsValidDynamicCode(Guid groupId, string secret, string code, DateTime nowUtc);
    int SecondsRemaining(DateTime atUtc);
    string CreateJoinUrl(string code);
    string NormalizeStaticCode(string code);
}
