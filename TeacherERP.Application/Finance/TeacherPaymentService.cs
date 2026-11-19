using System.Data;
using Hangfire;
using TeacherERP.Application.Common;
using TeacherERP.Application.Files;
using TeacherERP.Application.Interfaces;
using TeacherERP.Application.SessionContent;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Finance;

public sealed class TeacherPaymentService : ITeacherPaymentService
{
    private readonly IFinancialLedgerRepository _repository;
    private readonly IFinancialLedgerService _ledger;
    private readonly IFileStorage _fileStorage;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditRepository _auditRepository;
    private readonly IBackgroundJobClient _backgroundJobs;

    public TeacherPaymentService(
        IFinancialLedgerRepository repository,
        IFinancialLedgerService ledger,
        IFileStorage fileStorage,
        IUnitOfWork unitOfWork,
        IAuditRepository auditRepository,
        IBackgroundJobClient backgroundJobs)
    {
        _repository = repository;
        _ledger = ledger;
        _fileStorage = fileStorage;
        _unitOfWork = unitOfWork;
        _auditRepository = auditRepository;
        _backgroundJobs = backgroundJobs;
    }

    public async Task<TeacherPaymentSettingsDto?> GetSettingsAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        var settings = await _repository.GetTeacherPaymentSettingsAsync(tenantId, cancellationToken);
        return settings is null ? null : new TeacherPaymentSettingsDto(settings.InstaPayLink, settings.QrImageUrl);
    }

    public async Task<Result<TeacherPaymentSettingsDto>> UpdateSettingsAsync(
        Guid tenantId, Guid ownerId, string instaPayLink, Stream? qrImage, string? fileName, CancellationToken cancellationToken)
    {
        if (!Uri.TryCreate(instaPayLink, UriKind.Absolute, out var link)
            || link.Scheme != Uri.UriSchemeHttps)
            return Result<TeacherPaymentSettingsDto>.Failure(FinanceErrors.InvalidPaymentLink);

        var settings = await _repository.GetTeacherPaymentSettingsAsync(tenantId, cancellationToken);
        if (settings is null && qrImage is null)
            return Result<TeacherPaymentSettingsDto>.Failure(FinanceErrors.PaymentQrRequired);

        StoredFile? uploadedFile = null;
        if (qrImage is not null)
        {
            var uploaded = await UploadImageAsync(qrImage, fileName ?? "instapay-qr",
                $"teacher-erp/{tenantId:N}/payment-settings", cancellationToken);
            if (uploaded.IsFailure) return Result<TeacherPaymentSettingsDto>.Failure(uploaded.Error);
            uploadedFile = uploaded.Value!;
        }

        string? oldPublicId = null;
        try
        {
            if (settings is null)
            {
                settings = TeacherPaymentSettings.Create(tenantId, instaPayLink, uploadedFile!.Url, uploadedFile.PublicId);
                _repository.AddTeacherPaymentSettings(settings);
            }
            else
            {
                oldPublicId = settings.Update(instaPayLink, uploadedFile?.Url, uploadedFile?.PublicId);
            }

            _auditRepository.Add(AuditLog.Create(tenantId, ownerId, AuditAction.InstaPaySettingsUpdated, null,
                "InstaPay payment details updated."));
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            if (uploadedFile is not null) EnqueueDelete(uploadedFile.PublicId);
            throw;
        }

        if (!string.IsNullOrWhiteSpace(oldPublicId)) EnqueueDelete(oldPublicId);
        return Result<TeacherPaymentSettingsDto>.Success(
            new TeacherPaymentSettingsDto(settings.InstaPayLink, settings.QrImageUrl));
    }

    public async Task<Result<StudentPaymentDetails>> GetStudentPaymentDetailsAsync(
        Guid tenantId, Guid studentId, Guid sessionId, CancellationToken cancellationToken)
    {
        var options = await _ledger.GetOptionsAsync(tenantId, studentId, sessionId, cancellationToken);
        if (options.IsFailure) return Result<StudentPaymentDetails>.Failure(options.Error);

        var settings = await GetSettingsAsync(tenantId, cancellationToken);
        return Result<StudentPaymentDetails>.Success(new StudentPaymentDetails(options.Value!, settings));
    }

    public async Task<Result<Guid>> SubmitReceiptAsync(
        Guid tenantId, Guid studentId, Guid sessionId, PaymentType type, decimal amount,
        Stream file, string fileName, CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(type))
            return Result<Guid>.Failure(FinanceErrors.UnsupportedPaymentType);
        if (amount <= 0 || decimal.Round(amount, 2) != amount)
            return Result<Guid>.Failure(FinanceErrors.InvalidAmount);
        if (await _repository.GetTeacherPaymentSettingsAsync(tenantId, cancellationToken) is null)
            return Result<Guid>.Failure(FinanceErrors.PaymentSettingsRequired);

        var optionsResult = await _ledger.GetOptionsAsync(tenantId, studentId, sessionId, cancellationToken);
        if (optionsResult.IsFailure) return Result<Guid>.Failure(optionsResult.Error);
        var options = optionsResult.Value!;
        var validation = ValidateSubmission(options, type, amount);
        if (validation is not null) return Result<Guid>.Failure(validation);
        if (await _repository.HasOpenExternalPaymentAsync(tenantId, studentId, type, sessionId, cancellationToken))
            return Result<Guid>.Failure(FinanceErrors.PaymentAlreadyPending);

        var uploaded = await UploadImageAsync(file, fileName,
            $"teacher-erp/{tenantId:N}/payment-receipts", cancellationToken);
        if (uploaded.IsFailure) return Result<Guid>.Failure(uploaded.Error);
        var receipt = uploaded.Value!;

        try
        {
            var result = await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                if (await _repository.HasOpenExternalPaymentAsync(tenantId, studentId, type, sessionId, cancellationToken))
                    return Result<Guid>.Failure(FinanceErrors.PaymentAlreadyPending);

                var transaction = Transaction.CreateExternalPayment(tenantId, studentId, amount, type, sessionId,
                    receipt.Url, receipt.PublicId);
                _repository.AddPayment(transaction);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<Guid>.Success(transaction.Id);
            }, IsolationLevel.Serializable, cancellationToken);

            if (result.IsFailure) EnqueueDelete(receipt.PublicId);
            return result;
        }
        catch
        {
            EnqueueDelete(receipt.PublicId);
            throw;
        }
    }

    public Task<PagedResult<PendingExternalPaymentItem>> GetPendingPaymentsPageAsync(
        Guid tenantId, PageRequest page, CancellationToken cancellationToken)
        => _repository.GetPendingExternalPaymentsPageAsync(tenantId, page, cancellationToken);

    public Task<Result<CashPaymentResult>> ApproveAsync(
        Guid tenantId, Guid reviewerId, Guid transactionId, CancellationToken cancellationToken)
        => _ledger.ApproveExternalPaymentAsync(tenantId, reviewerId, transactionId, cancellationToken);

    public Task<Result<bool>> RejectAsync(
        Guid tenantId, Guid reviewerId, Guid transactionId, CancellationToken cancellationToken)
        => _ledger.RejectExternalPaymentAsync(tenantId, reviewerId, transactionId, cancellationToken);

    private static Error? ValidateSubmission(FinancialPaymentOptions options, PaymentType type, decimal amount)
    {
        var otherDebts = options.Debts.Where(x => x.SessionId != options.SessionId && x.OutstandingAmount > 0).ToList();
        switch (type)
        {
            case PaymentType.DebtSettlement:
                return options.TotalOutstanding > 0 ? null : FinanceErrors.NoOutstandingDebt;
            case PaymentType.PerSession:
                if (otherDebts.Count > 0) return FinanceErrors.PreviousDebtMustBeSettled;
                return options.Debts.Any(x => x.SessionId == options.SessionId && x.OutstandingAmount > 0)
                    ? null : FinanceErrors.SessionChargeNotFound;
            case PaymentType.MonthlySubscription:
                if (otherDebts.Count > 0) return FinanceErrors.PreviousDebtMustBeSettled;
                if (options.MonthlyPrice is not { } monthlyPrice) return FinanceErrors.MonthlyPriceNotConfigured;
                if (options.HasMonthlySubscription) return FinanceErrors.SubscriptionAlreadyExists;
                return amount + options.WalletCredit >= monthlyPrice ? null : FinanceErrors.InsufficientPayment;
            default:
                return FinanceErrors.UnsupportedPaymentType;
        }
    }

    private async Task<Result<StoredFile>> UploadImageAsync(
        Stream source, string fileName, string folder, CancellationToken cancellationToken)
    {
        if (source is null) return Result<StoredFile>.Failure(FinanceErrors.PaymentReceiptRequired);
        Stream stream = source;
        MemoryStream? copy = null;
        if (!stream.CanSeek)
        {
            copy = new MemoryStream();
            await stream.CopyToAsync(copy, cancellationToken);
            copy.Position = 0;
            stream = copy;
        }

        try
        {
            if (stream.Length == 0 || stream.Length > SessionContentPolicy.MaxFileBytes)
                return Result<StoredFile>.Failure(FinanceErrors.PaymentImageInvalid);

            var header = new byte[FileSignature.HeaderLength];
            var read = 0;
            while (read < header.Length)
            {
                var count = await stream.ReadAsync(header.AsMemory(read), cancellationToken);
                if (count == 0) break;
                read += count;
            }
            stream.Position = 0;
            var type = FileSignature.Detect(header.AsSpan(0, read));
            if (type is not (DetectedFileType.Jpeg or DetectedFileType.Png or DetectedFileType.Webp))
                return Result<StoredFile>.Failure(FinanceErrors.PaymentReceiptInvalid);

            var safeName = $"payment-{Guid.NewGuid():N}{FileSignature.ExtensionOf(type)}";
            return await _fileStorage.UploadAsync(stream, safeName, folder, cancellationToken);
        }
        finally
        {
            if (copy is not null) await copy.DisposeAsync();
        }
    }

    private void EnqueueDelete(string publicId)
        => _backgroundJobs.Enqueue<IStoredFileCleanupJob>(job => job.DeleteAsync(publicId));
}
