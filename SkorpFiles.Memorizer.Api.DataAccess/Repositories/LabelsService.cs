using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SkorpFiles.Memorizer.Api.DataAccess.Models;
using SkorpFiles.Memorizer.Api.DataAccess.Interfaces;

namespace SkorpFiles.Memorizer.Api.DataAccess.Repositories
{
    public class LabelsService: ILabelsService
    {
        private const int MaxAttempts = 100;

        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;

        public LabelsService(
            IDbContextFactory<ApplicationDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Dictionary<string, Guid>> EnsureLabelsAsync(
            IEnumerable<string> requestedNames,
            CancellationToken cancellationToken = default)
        {
            if (requestedNames.Count() == 0)
            {
                return [];
            }

            var normalizedNames = requestedNames.Distinct().ToDictionary(
                name => name,
                NormalizeLabelName);

            for (var attempt = 1; attempt <= MaxAttempts; attempt++)
            {
                await using var dbContext =
                    await _contextFactory.CreateDbContextAsync(cancellationToken);

                var existingLabels = await dbContext.NormalizedLabels
                    .Where(x => normalizedNames.Values.Contains(x.NormalizedLabelName))
                    .Select(x => new
                    {
                        x.NormalizedLabelId,
                        x.NormalizedLabelName
                    })
                    .ToDictionaryAsync(
                        x => x.NormalizedLabelName,
                        x => x.NormalizedLabelId,
                        cancellationToken);

                var newLabels = normalizedNames.Values
                    .Where(x => !existingLabels.ContainsKey(x))
                    .Select(x => new NormalizedLabel
                    {
                        NormalizedLabelName = x
                    })
                    .ToArray();

                var result = new Dictionary<string, Guid>();

                foreach (var existingLabel in existingLabels)
                {
                    result.Add(normalizedNames.First(x => x.Value == existingLabel.Key).Key, existingLabel.Value);
                }

                if (newLabels.Length == 0)
                {
                    return result;
                }

                dbContext.NormalizedLabels.AddRange(newLabels);

                try
                {
                    await dbContext.SaveChangesAsync(cancellationToken);

                    foreach (var newLabel in newLabels)
                    {
                        result.Add(normalizedNames.First(x=>x.Value == newLabel.NormalizedLabelName).Key, newLabel.NormalizedLabelId);
                    }

                    return result;
                }
                catch (DbUpdateException exception)
                    when (IsUniqueConstraintViolation(exception)
                          && attempt < MaxAttempts)
                { }
            }

            throw new InvalidOperationException(
                "Unable to ensure that labels exist.");
        }

        private static bool IsUniqueConstraintViolation(
            DbUpdateException exception)
        {
            return exception.InnerException is SqlException
            {
                Number: 2601 or 2627
            };
        }

        private static string NormalizeLabelName(string name)
        {
            return name.Trim().ToUpperInvariant();
        }
    }
}
