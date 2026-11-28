// جديد — TeacherERP.Application/Common/IPagedRequest.cs
namespace TeacherERP.Application.Common
{
    // أي استعلام قائمة بصفحات: الصفحة الأولى = 1
    public interface IPagedRequest
    {
        int Page { get; }
        int PageSize { get; }
    }
}