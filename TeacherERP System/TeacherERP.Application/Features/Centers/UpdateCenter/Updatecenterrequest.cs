namespace TeacherERP.Application.Features.Centers.UpdateCenter
{
    // جسم الطلب (الـ Id يأتي من المسار)، استبدال كامل لبيانات السنتر
    public record UpdateCenterRequest(
        string Name,
        string? Address,
        double? Latitude,
        double? Longitude,
        int GeofenceRadiusMeters);
}