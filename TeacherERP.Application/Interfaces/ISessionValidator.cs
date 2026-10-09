namespace TeacherERP.Application.Interfaces
{
    // فحص صلاحية الجلسة عند كل طلب (مع كاش قصير يُمسح فور الإلغاء)
    public interface ISessionValidator
    {
        Task<bool> IsActiveAsync(Guid sessionId, CancellationToken cancellationToken);
        void Invalidate(Guid sessionId);
    }
}