// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using RebacExperiments.Server.Database.Models;

namespace RebacExperiments.Server.Api.Services
{
    public interface ILocalizationRecordService
    {
        Task<LocalizationRecord> CreateLocalizationRecordAsync(LocalizationRecord localizationRecord, int currentUserId, CancellationToken cancellationToken);
        Task DeleteLocalizationRecordAsync(int LocalizationRecordId, int currentUserId, CancellationToken cancellationToken);
        IQueryable<LocalizationRecord> GetAllLocalizationRecordsQueryable();
        Task<LocalizationRecord> GetLocalizationRecordByIdAsync(int localizationRecordId, int currentUserId, CancellationToken cancellationToken);
        Task<LocalizationRecord> UpdateLocalizationRecordAsync(LocalizationRecord localizationRecord, int currentUserId, CancellationToken cancellationToken);
    }
}