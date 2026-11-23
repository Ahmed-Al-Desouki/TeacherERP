using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Features.Groups.JoinCodes;

public sealed record GroupJoinCodesDto(
    Guid GroupId,
    bool StaticEnabled,
    string? StaticCode,
    string? StaticJoinUrl,
    bool DynamicEnabled,
    string? DynamicCode,
    string? DynamicJoinUrl,
    int? DynamicExpiresInSeconds,
    DateTime? DynamicExpiresAtUtc)
{
    public static GroupJoinCodesDto From(Group group, IGroupJoinCodeService codeService, DateTime nowUtc)
    {
        var staticCode = group.StaticJoinEnabled ? group.StaticJoinCode : null;
        var dynamicCode = group.DynamicJoinEnabled && group.DynamicJoinSecret is not null
            ? codeService.CreateDynamicCode(group.Id, group.DynamicJoinSecret, nowUtc)
            : null;
        int? expiresIn = dynamicCode is null ? null : codeService.SecondsRemaining(nowUtc);

        return new GroupJoinCodesDto(
            group.Id,
            group.StaticJoinEnabled,
            staticCode,
            staticCode is null ? null : codeService.CreateJoinUrl(staticCode),
            group.DynamicJoinEnabled,
            dynamicCode,
            dynamicCode is null ? null : codeService.CreateJoinUrl(dynamicCode),
            expiresIn,
            expiresIn is null ? null : nowUtc.AddSeconds(expiresIn.Value));
    }
}
