// جديد — TeacherERP.Application/Features/TeacherRequests/Verify/VerifyTeacherRequestCommandHandler.cs
using System.Data;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.TeacherRequests.Verify
{
    public class VerifyTeacherRequestCommandHandler : IRequestHandler<VerifyTeacherRequestCommand, Result<TeacherRequestQrDto>>
    {
        private readonly IMemoryCache _cache;
        private readonly IOtpService _otpService;
        private readonly ITeacherAccessRequestRepository _requestRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeProvider _timeProvider;

        public VerifyTeacherRequestCommandHandler(
            IMemoryCache cache,
            IOtpService otpService,
            ITeacherAccessRequestRepository requestRepository,
            IUnitOfWork unitOfWork,
            TimeProvider timeProvider)
        {
            _cache = cache;
            _otpService = otpService;
            _requestRepository = requestRepository;
            _unitOfWork = unitOfWork;
            _timeProvider = timeProvider;
        }

        public async Task<Result<TeacherRequestQrDto>> Handle(VerifyTeacherRequestCommand request, CancellationToken cancellationToken)
        {
            var email = request.Email.Trim().ToLowerInvariant();
            var cacheKey = TeacherRequestCacheKey.For(email);

            // 1. البيانات المعلقة أولاً (قبل الكود) حتى لا يُستهلك كود بلا داعٍ. غيابها = لم يُرسل أو إيميل غير مسموح
            if (!_cache.TryGetValue(cacheKey, out CachedTeacherRequestData? data) || data is null)
            {
                return Result<TeacherRequestQrDto>.Failure(TeacherRequestErrors.SubmissionExpired);
            }

            // 2. الكود (يشمل حد المحاولات والقفل)
            var otpResult = await _otpService.VerifyAsync(OtpPurpose.TeacherRequest, email, request.OtpCode, cancellationToken);
            if (otpResult.IsFailure)
            {
                return Result<TeacherRequestQrDto>.Failure(otpResult.Error);
            }

            // 3. سر جديد للـ QR (نخزّن بصمته فقط)
            var token = TeacherRequestToken.Generate();
            var now = _timeProvider.GetUtcNow().UtcDateTime;
            var expiresAt = now + TeacherRequestPolicy.RequestLifetime;

            // 4. طلب معلّق واحد لكل إيميل: أي طلب سابق يُنهى ثم يُنشأ الجديد
            // (حفظان منفصلان داخل Transaction واحدة، حتى لا يتعارض الفهرس الفريد المرشَّح أثناء الاستبدال)
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var previous = await _requestRepository.GetPendingByEmailAsync(email, cancellationToken);
                if (previous.Count > 0)
                {
                    foreach (var old in previous)
                    {
                        old.MarkExpired();
                    }

                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }

                _requestRepository.Add(TeacherAccessRequest.Create(
                    data.FullName, data.Email, data.Subject, data.Note, data.TimeZoneId,
                    TeacherRequestToken.Hash(token), expiresAt));
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return true;
            }, IsolationLevel.ReadCommitted, cancellationToken);

            _cache.Remove(cacheKey);

            return Result<TeacherRequestQrDto>.Success(new TeacherRequestQrDto(token, expiresAt));
        }
    }
}
