// جديد — TeacherERP.Application/Common/PageRequest.cs
namespace TeacherERP.Application.Common
{
    // طلب صفحة بعد التحقق منه (القيم مفحوصة في PagedRequestRules)
    public sealed record PageRequest(int Page, int PageSize)
    {
        public const int DefaultPageSize = 20;
        public const int MaxPageSize = 100;

        public int Skip => (Page - 1) * PageSize;

        public static PageRequest From(IPagedRequest request) => new(request.Page, request.PageSize);
    }
}