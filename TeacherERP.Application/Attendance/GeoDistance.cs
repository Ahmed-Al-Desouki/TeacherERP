// جديد — TeacherERP.Application/Attendance/GeoDistance.cs
namespace TeacherERP.Application.Attendance
{
    public static class GeoDistance
    {
        private const double EarthRadiusMeters = 6_371_000;

        // المسافة بالأمتار بين نقطتين (Haversine)
        public static double Meters(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            var dLat = ToRadians(latitude2 - latitude1);
            var dLon = ToRadians(longitude2 - longitude1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                    + Math.Cos(ToRadians(latitude1)) * Math.Cos(ToRadians(latitude2))
                    * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            return EarthRadiusMeters * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }

        private static double ToRadians(double degrees) => degrees * Math.PI / 180;
    }
}