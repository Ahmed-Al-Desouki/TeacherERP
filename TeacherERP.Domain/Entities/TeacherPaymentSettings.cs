namespace TeacherERP.Domain.Entities;

/// <summary>وسيلة الدفع الخارجية التي يعرضها المدرس للطلاب داخل مساحته.</summary>
public sealed class TeacherPaymentSettings : BaseEntity, ITenantOwned
{
    public Guid TenantId { get; private set; }
    public string InstaPayLink { get; private set; } = string.Empty;
    public string QrImageUrl { get; private set; } = string.Empty;
    public string QrImagePublicId { get; private set; } = string.Empty;

    private TeacherPaymentSettings() { }

    public static TeacherPaymentSettings Create(Guid tenantId, string instaPayLink, string qrImageUrl, string qrImagePublicId)
    {
        return new TeacherPaymentSettings
        {
            TenantId = tenantId,
            InstaPayLink = instaPayLink.Trim(),
            QrImageUrl = qrImageUrl,
            QrImagePublicId = qrImagePublicId
        };
    }

    public string? Update(string instaPayLink, string? qrImageUrl, string? qrImagePublicId)
    {
        InstaPayLink = instaPayLink.Trim();
        var oldPublicId = QrImagePublicId;
        if (!string.IsNullOrWhiteSpace(qrImageUrl) && !string.IsNullOrWhiteSpace(qrImagePublicId))
        {
            QrImageUrl = qrImageUrl;
            QrImagePublicId = qrImagePublicId;
        }
        MarkAsUpdated();
        return QrImagePublicId == oldPublicId ? null : oldPublicId;
    }
}
