namespace TeacherERP.Application.Interfaces
{
    public interface IUserTimeZoneAccessor
    {
        // منطقة الطلب من X-Time-Zone، أو تفضيل المستخدم من JWT عند غياب الترويسة، أو null عند عدم توفرهما.
        string? TimeZoneId { get; }
    }
}
