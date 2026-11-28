using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Administration.CreateTeacher
{
    // مدير النظام يضيف مدرساً بإيميله: حساب بباسورد مؤقت + مساحته الخاصة. التوقيت اختياري (الافتراضي القاهرة)
    public record CreateTeacherCommand(string FullName, string Email, string? TimeZoneId = null)
        : IRequest<Result<CreatedTeacherDto>>;
}
