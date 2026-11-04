// جديد — TeacherERP.Application/Features/GroupAdmins/GroupScan/GroupScanCommand.cs
using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.GroupAdmins.GroupScan
{
    // الطالب المسؤول يمسح كارت طالب في الحصة الجارية لمجموعته فقط. الموقع هو موقع الأدمن نفسه (حتى لا يسجّل أصحابه من بعيد)
    public record GroupScanCommand(string StudentQrCode, double Latitude, double Longitude, bool IsMockLocation)
        : IRequest<Result<AttendanceResultDto>>;
}