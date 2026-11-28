// جديد — TeacherERP.Application/Features/Groups/IGroupSchedule.cs
namespace TeacherERP.Application.Features.Groups
{
    // أي أمر فيه جدول أسبوعي كامل (إنشاء مجموعة أو تعديل جدولها) تنطبق عليه نفس قواعد التحقق
    public interface IGroupSchedule
    {
        List<GroupSlotInput> Slots { get; }
    }
}