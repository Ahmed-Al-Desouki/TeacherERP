// موجود (تعديل) — TeacherERP.Application/Features/Directory/SearchDirectory/SearchDirectoryQueryHandler.cs
using MediatR;
using TeacherERP.Application.Common;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Directory.SearchDirectory
{
    public class SearchDirectoryQueryHandler : IRequestHandler<SearchDirectoryQuery, Result<PagedResult<DirectoryCenterDto>>>
    {
        private const int MinTermLength = 2;
        private const int MaxTermLength = 50;

        private readonly IDirectoryRepository _directoryRepository;

        public SearchDirectoryQueryHandler(IDirectoryRepository directoryRepository)
        {
            _directoryRepository = directoryRepository;
        }

        public async Task<Result<PagedResult<DirectoryCenterDto>>> Handle(SearchDirectoryQuery request, CancellationToken cancellationToken)
        {
            var term = request.Term?.Trim() ?? string.Empty;
            if (term.Length < MinTermLength || term.Length > MaxTermLength)
            {
                return Result<PagedResult<DirectoryCenterDto>>.Failure(new Error(
                    "Directory.InvalidTerm", $"The search text must be between {MinTermLength} and {MaxTermLength} characters."));
            }

            var found = await _directoryRepository.SearchAsync(term, PageRequest.From(request), cancellationToken);

            // الصفوف مرتبة بالسنتر أولاً، فالتجميع يحافظ على الترتيب
            var centers = found.Rows
                .GroupBy(row => row.CenterId)
                .Select(group =>
                {
                    var first = group.First();
                    return new DirectoryCenterDto(
                        first.CenterId,
                        first.CenterName,
                        first.CenterAddress,
                        group.Select(row => new DirectoryTeacherDto(row.TeacherId, row.TeacherName, row.Subject)).ToList());
                })
                .ToList();

            return Result<PagedResult<DirectoryCenterDto>>.Success(
                new PagedResult<DirectoryCenterDto>(centers, request.Page, request.PageSize, found.TotalCenters));
        }
    }
}
