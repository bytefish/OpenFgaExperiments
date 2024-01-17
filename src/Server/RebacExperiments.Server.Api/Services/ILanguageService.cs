// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using RebacExperiments.Server.Database.Models;

namespace RebacExperiments.Server.Api.Services
{
    public interface ILanguageService
    {
        Task<Language> CreateLanguageAsync(Language language, int currentUserId, CancellationToken cancellationToken);
        Task DeleteLanguageAsync(int languageId, int currentUserId, CancellationToken cancellationToken);
        Task<List<Language>> GetAllLanguagesAsync(CancellationToken cancellationToken);
        Task<Language> GetLanguageByIdAsync(int languageId, int currentUserId, CancellationToken cancellationToken);
        Task<Language> UpdateLanguageAsync(Language language, int currentUserId, CancellationToken cancellationToken);
    }
}