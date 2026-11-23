// جديد — TeacherERP.Application/Interfaces/IPasswordGenerator.cs
namespace TeacherERP.Application.Interfaces
{
    // توليد باسورد مؤقت عشوائي (يحقق سياسة الـ Identity) لحسابات المساعدين التي ينشئها المدرس
    public interface IPasswordGenerator
    {
        string Generate();
    }
}