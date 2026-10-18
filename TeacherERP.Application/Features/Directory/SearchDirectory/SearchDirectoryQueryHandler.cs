// جديد — TeacherERP.Application/Features/Directory/SearchDirectory/SearchDirectoryQueryHandler.cs
using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Directory.SearchDirectory
{
    public class SearchDirectoryQueryHandler : IRequestHandler<SearchDirectoryQuery, Result<List<DirectoryCenterDto>>>
    {
        private const int MinTermLength = 2;
        private const int MaxTermLength = 50;
        private const int MaxCenters = 20;
        private const int MaxRows = 200;

        private readonly IDirectoryRepository _directoryRepository;

        public SearchDirectoryQueryHandler(IDirectoryRepository directoryRepository)
        {
            _directoryRepository = directoryRepository;
        }

        public async Task<Result<List<DirectoryCenterDto>>> Handle(SearchDirectoryQuery request, CancellationToken cancellationToken)
        {
            var term = request.Term?.Trim() ?? string.Empty;
            if (term.Length < MinTermLength || term.Length > MaxTermLength)
            {
                return Result<List<DirectoryCenterDto>>.Failure(new Error(
                    "Directory.InvalidTerm", $"The search text must be between {MinTermLength} and {MaxTermLength} characters."));
            }

            var rows = await _directoryRepository.SearchAsync(term, MaxRows, cancellationToken);

            var centers = rows
                .GroupBy(row => row.CenterId)
                .Take(MaxCenters)
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

            return Result<List<DirectoryCenterDto>>.Success(centers);
        }
    }
}