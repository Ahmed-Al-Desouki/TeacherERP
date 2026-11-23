namespace TeacherERP.Application.Interfaces
{
    // بيانات المستخدم والجلسة الحالية من توكن الطلب (null لو الطلب غير مصادَق)
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        Guid? SessionId { get; }
    }
}