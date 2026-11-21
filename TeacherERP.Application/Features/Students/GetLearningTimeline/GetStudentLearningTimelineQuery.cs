using MediatR;
using TeacherERP.Application.Common;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Students.GetLearningTimeline;

public sealed record GetStudentLearningTimelineQuery(
    DateTime? From = null,
    DateTime? To = null,
    Guid? GroupId = null,
    int Page = 1,
    int PageSize = PageRequest.DefaultPageSize)
    : IRequest<Result<PagedResult<StudentTimelineSessionDto>>>, IPagedRequest;
