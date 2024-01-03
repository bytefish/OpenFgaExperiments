// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using RebacExperiments.Server.Api.Infrastructure.Exceptions;
using RebacExperiments.Server.Api.Infrastructure.Logging;
using RebacExperiments.Server.Database;
using RebacExperiments.Server.Database.Models;

namespace RebacExperiments.Server.Api.Services
{
    public class LocalizationRecordService : ILocalizationRecordService
    {
        private readonly ILogger<LocalizationRecordService> _logger;

        private readonly ApplicationDbContext _applicationDbContext;

        public LocalizationRecordService(ILogger<LocalizationRecordService> logger, ApplicationDbContext applicationDbContext)
        {
            _logger = logger;
            _applicationDbContext = applicationDbContext;
        }

        public async Task<LocalizationRecord> CreateLocalizationRecordAsync(LocalizationRecord localizationRecord, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            // Make sure the Current User is the last editor:
            localizationRecord.LastEditedBy = currentUserId;

            // Add the new Task, the HiLo Pattern automatically assigns a new Id using the HiLo Pattern
            await _applicationDbContext
                .AddAsync(localizationRecord, cancellationToken)
                .ConfigureAwait(false);

            await _applicationDbContext
                .SaveChangesAsync(cancellationToken)
                .ConfigureAwait(false);

            return localizationRecord;
        }

        public IQueryable<LocalizationRecord> GetAllLocalizationRecordsQueryable()
        {
            _logger.TraceMethodEntry();

            return _applicationDbContext.LocalizationRecords.AsNoTracking();
        }

        public async Task<LocalizationRecord> GetLocalizationRecordByIdAsync(int localizationRecordId, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            var localizationRecord = await _applicationDbContext.LocalizationRecords
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == localizationRecordId, cancellationToken);

            if (localizationRecord == null)
            {
                throw new EntityNotFoundException()
                {
                    EntityName = nameof(LocalizationRecord),
                    EntityId = localizationRecordId,
                };
            }

            return localizationRecord;
        }

        public async Task<LocalizationRecord> UpdateLocalizationRecordAsync(LocalizationRecord localizationRecord, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            int rowsAffected = await _applicationDbContext.LocalizationRecords
                .Where(t => t.Id == localizationRecord.Id && t.RowVersion == localizationRecord.RowVersion)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.Name, localizationRecord.Name)
                    .SetProperty(x => x.LanguageId, localizationRecord.LanguageId)
                    .SetProperty(x => x.Value, localizationRecord.Value)
                    .SetProperty(x => x.Category, localizationRecord.Category)
                    .SetProperty(x => x.LastEditedBy, currentUserId), cancellationToken);

            if (rowsAffected == 0)
            {
                throw new EntityConcurrencyException()
                {
                    EntityName = nameof(LocalizationRecord),
                    EntityId = localizationRecord.Id,
                };
            }

            return localizationRecord;

        }

        public async Task DeleteLocalizationRecordAsync(int LocalizationRecordId, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            var LocalizationRecord = await _applicationDbContext.LocalizationRecords
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == LocalizationRecordId, cancellationToken);

            if (LocalizationRecord == null)
            {
                throw new EntityNotFoundException()
                {
                    EntityName = nameof(LocalizationRecord),
                    EntityId = LocalizationRecordId,
                };
            }

            await _applicationDbContext.LocalizationRecords
                    .Where(t => t.Id == LocalizationRecordId)
                    .ExecuteDeleteAsync(cancellationToken)
                    .ConfigureAwait(false);
        }
    }
}