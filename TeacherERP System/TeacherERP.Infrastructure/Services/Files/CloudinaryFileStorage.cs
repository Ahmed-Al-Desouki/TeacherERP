// جديد — TeacherERP.Infrastructure/Services/CloudinaryFileStorage.cs
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TeacherERP.Application.Files;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;
using TeacherERP.Infrastructure.Settings;

namespace TeacherERP.Infrastructure.Services
{
    // Singleton: العميل آمن للاستخدام المتزامن. الإعدادات الناقصة لا توقف التطبيق، بل ترجّع FileStorage.NotConfigured عند أول استخدام.
    // كل الملفات (صور وPDF) تُخزَّن كنوع image، فالحذف بالـ PublicId وحده يكفي.
    public class CloudinaryFileStorage : IFileStorage
    {
        private readonly ILogger<CloudinaryFileStorage> _logger;
        private readonly Lazy<Cloudinary?> _client;

        public CloudinaryFileStorage(IOptions<CloudinarySettings> settings, ILogger<CloudinaryFileStorage> logger)
        {
            _logger = logger;

            var value = settings.Value;
            _client = new Lazy<Cloudinary?>(() =>
                string.IsNullOrWhiteSpace(value.CloudName)
                || string.IsNullOrWhiteSpace(value.ApiKey)
                || string.IsNullOrWhiteSpace(value.ApiSecret)
                    ? null
                    : new Cloudinary(new Account(value.CloudName, value.ApiKey, value.ApiSecret)));
        }

        public async Task<Result<StoredFile>> UploadAsync(
            Stream content, string fileName, string folder, CancellationToken cancellationToken)
        {
            var client = _client.Value;
            if (client is null)
            {
                _logger.LogError("CloudinarySettings is not configured.");
                return Result<StoredFile>.Failure(FileStorageErrors.NotConfigured);
            }

            try
            {
                var parameters = new ImageUploadParams
                {
                    File = new FileDescription(fileName, content),
                    Folder = folder,
                    // اسم عشوائي لا يُخمَّن، ولا نستخدم اسم الملف الأصلي في العنوان أبداً
                    PublicId = Guid.NewGuid().ToString("N")
                };

                var result = await client.UploadAsync(parameters, cancellationToken);

                if (result.Error is not null || result.SecureUrl is null || string.IsNullOrEmpty(result.PublicId))
                {
                    _logger.LogError("Cloudinary upload failed in folder {Folder}: {Message}", folder, result.Error?.Message);
                    return Result<StoredFile>.Failure(FileStorageErrors.UploadFailed);
                }

                return Result<StoredFile>.Success(new StoredFile(result.SecureUrl.ToString(), result.PublicId));
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Cloudinary upload threw in folder {Folder}.", folder);
                return Result<StoredFile>.Failure(FileStorageErrors.UploadFailed);
            }
        }

        public async Task<Result<bool>> DeleteAsync(string publicId, CancellationToken cancellationToken)
        {
            var client = _client.Value;
            if (client is null)
            {
                _logger.LogError("CloudinarySettings is not configured.");
                return Result<bool>.Failure(FileStorageErrors.NotConfigured);
            }

            try
            {
                var result = await client.DestroyAsync(new DeletionParams(publicId));

                // "not found" ليس خطأً: الهدف أن الملف لم يعد موجوداً
                if (result.Error is not null)
                {
                    _logger.LogError("Cloudinary delete failed for {PublicId}: {Message}", publicId, result.Error.Message);
                    return Result<bool>.Failure(FileStorageErrors.DeleteFailed);
                }

                return Result<bool>.Success(true);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Cloudinary delete threw for {PublicId}.", publicId);
                return Result<bool>.Failure(FileStorageErrors.DeleteFailed);
            }
        }

        public async Task<FileStorageResourcePage> ListApplicationFilesAsync(
            string? nextCursor, int pageSize, CancellationToken cancellationToken)
        {
            var client = _client.Value;
            if (client is null)
                throw new InvalidOperationException("Cloudinary file listing is unavailable because storage is not configured.");

            // ListResourcesByPrefix only covers legacy public-ID folder paths. Listing a page
            // and filtering both asset_folder and public_id also supports Cloudinary dynamic folders.
            var parameters = new ListResourcesParams
            {
                Type = "upload",
                ResourceType = ResourceType.Image,
                MaxResults = Math.Clamp(pageSize, 1, 100),
                NextCursor = nextCursor,
                Fields = new[] { "asset_folder" }
            };

            var result = await client.ListResourcesAsync(parameters, cancellationToken);
            if (result.Error is not null)
            {
                _logger.LogError("Cloudinary resource listing failed for application assets: {Message}", result.Error.Message);
                throw new InvalidOperationException("Cloudinary resource listing failed.");
            }

            var resources = result.Resources
                .Where(resource => !string.IsNullOrWhiteSpace(resource.PublicId)
                    && (resource.AssetFolder?.StartsWith("teacher-erp/", StringComparison.Ordinal) == true
                        || resource.PublicId.StartsWith("teacher-erp/", StringComparison.Ordinal)))
                .Select(resource => new FileStorageResource(resource.PublicId))
                .ToArray();
            return new FileStorageResourcePage(resources, result.NextCursor);
        }
    }
}
