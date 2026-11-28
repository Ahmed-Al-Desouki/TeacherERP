// جديد — TeacherERP.Application/Common/PagedResult.cs
namespace TeacherERP.Application.Common
{
    // الشكل الموحد لأي قائمة بصفحات في الـ API
    public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount)
    {
        public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);

        public bool HasPrevious => Page > 1;

        public bool HasNext => Page < TotalPages;

        public PagedResult<TOut> Map<TOut>(Func<T, TOut> selector)
            => new(Items.Select(selector).ToList(), Page, PageSize, TotalCount);
    }
}