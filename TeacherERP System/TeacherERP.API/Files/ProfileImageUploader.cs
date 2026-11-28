using Microsoft.AspNetCore.Http;
using TeacherERP.Application.Interfaces;
using TeacherERP.Application.SessionContent;
using TeacherERP.Domain.Shared;

namespace TeacherERP.API.Files;

internal static class ProfileImageUploader
{
    private const long MaxFileBytes = 5 * 1024 * 1024;

    public static async Task<Result<StoredFile>> UploadAsync(
        IFormFile file, IFileStorage storage, CancellationToken cancellationToken)
    {
        if (file.Length == 0 || file.Length > MaxFileBytes)
        {
            return Result<StoredFile>.Failure(new Error("Profile.InvalidImage", "Profile image must be an image up to 5 MB."));
        }

        await using var stream = file.OpenReadStream();
        var header = new byte[FileSignature.HeaderLength];
        var read = await stream.ReadAsync(header.AsMemory(), cancellationToken);
        var type = FileSignature.Detect(header.AsSpan(0, read));
        if (type is not (DetectedFileType.Jpeg or DetectedFileType.Png or DetectedFileType.Webp))
        {
            return Result<StoredFile>.Failure(new Error("Profile.InvalidImage", "Profile image must be a JPEG, PNG, or WebP image up to 5 MB."));
        }

        stream.Position = 0;
        return await storage.UploadAsync(stream, $"profile{FileSignature.ExtensionOf(type)}",
            "teacher-erp/profile-images", cancellationToken);
    }
}
