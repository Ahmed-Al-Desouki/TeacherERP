// موجود (تعديل) — TeacherERP.Application/Features/Attendance/AttendanceDtos.cs
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Features.Attendance
{
    // محتوى الـ QR الثابت للطباعة: التطبيق يبني منه الكود (teacherId + code)
    public record AttendanceSettingsDto(
        Guid TeacherId,
        AttendanceMode Mode,
        bool RejectMockLocations,
        bool AutoRegisterOnScan,
        string? StaticCode,
        bool AutoApproveGroupAdminScans)
    {
        // السر لا يُعرض إلا في الوضع الثابت (للطباعة)، وفي المتغير لا حاجة لعرضه أبداً
        public static AttendanceSettingsDto From(
            Guid tenantId,
            AttendanceMode mode,
            bool rejectMockLocations,
            bool autoRegisterOnScan,
            bool autoApproveGroupAdminScans,
            string qrSecret)
            => new(
                tenantId,
                mode,
                rejectMockLocations,
                autoRegisterOnScan,
                mode == AttendanceMode.StaticGlobalQR ? qrSecret : null,
                autoApproveGroupAdminScans);
    }

    // الكود المتغير لشاشة المدرس (يُجدَّد قبل انتهاء ExpiresInSeconds)
    public record DisplayCodeDto(Guid TeacherId, string Code, int ExpiresInSeconds);
}