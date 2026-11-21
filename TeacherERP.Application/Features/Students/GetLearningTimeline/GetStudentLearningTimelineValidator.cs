using FluentValidation;
using TeacherERP.Application.Common;

namespace TeacherERP.Application.Features.Students.GetLearningTimeline;

public sealed class GetStudentLearningTimelineValidator : AbstractValidator<GetStudentLearningTimelineQuery>
{
    public GetStudentLearningTimelineValidator() => Include(new PagedRequestRules<GetStudentLearningTimelineQuery>());
}
