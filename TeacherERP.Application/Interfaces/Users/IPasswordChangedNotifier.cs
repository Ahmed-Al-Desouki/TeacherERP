using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces;

public interface IPasswordChangedNotifier
{
    void Enqueue(ApplicationUser user);
}
