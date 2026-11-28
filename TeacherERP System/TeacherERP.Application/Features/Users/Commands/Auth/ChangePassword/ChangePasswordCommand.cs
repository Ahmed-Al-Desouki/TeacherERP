// جديد — TeacherERP.Application/Features/Users/Commands/Auth/ChangePassword/ChangePasswordCommand.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Users.Commands.Auth.ChangePassword
{
    // الكلام دا لل assistant أو teacher اللي عايز يغير الباسورد بتاعه، لازم يكون مسجّل دخوله بالباسورد الحالي
    // تغيير الباسورد للمسجّل دخوله بالباسورد الحالي، وتنتهي باقي أجهزته (الجهاز الحالي يفضل مسجّلاً)
    public record ChangePasswordCommand(string CurrentPassword, string NewPassword) : IRequest<Result<bool>>;
}