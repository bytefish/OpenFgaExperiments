// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using RebacExperiments.Server.Api.Infrastructure.Constants;
using RebacExperiments.Server.Api.Infrastructure.Exceptions;
using RebacExperiments.Server.Api.Infrastructure.Logging;
using RebacExperiments.Server.Database;
using RebacExperiments.Server.Database.Models;

namespace RebacExperiments.Server.Api.Services
{
    public class LanguageService : ILanguageService
    {
        private readonly ILogger<LanguageService> _logger;

        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IAclService _aclService;

        public LanguageService(ILogger<LanguageService> logger, ApplicationDbContext applicationDbContext, IAclService aclService)
        {
            _logger = logger;
            _applicationDbContext = applicationDbContext;
            _aclService = aclService;
        }

        public async Task<Language> CreateLanguageAsync(Language language, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            // Make sure the Current User is the last editor:
            language.LastEditedBy = currentUserId;

            // Add the new Task, the HiLo Pattern automatically assigns a new Id using the HiLo Pattern
            await _applicationDbContext
                .AddAsync(language, cancellationToken)
                .ConfigureAwait(false);

            return language;
        }

        public async Task<Language> GetLanguageByIdAsync(int languageId, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            var language = await _applicationDbContext.Languages
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == languageId, cancellationToken);

            if (language == null)
            {
                throw new EntityNotFoundException()
                {
                    EntityName = nameof(Language),
                    EntityId = languageId,
                };
            }

            return language;
        }

        public async Task<List<Language>> GetAllLanguagesAsync(CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            var languages = await _applicationDbContext.Languages
                .AsNoTracking()
                .ToListAsync(cancellationToken).ConfigureAwait(false);

            return languages;
        }

        public async Task<Language> UpdateLanguageAsync(Language language, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            bool isAuthorized = await _aclService.CheckUserObjectAsync(currentUserId, language, Actions.CanWrite, cancellationToken);

            if (!isAuthorized)
            {
                throw new EntityUnauthorizedAccessException()
                {
                    EntityName = nameof(Language),
                    EntityId = language.Id,
                    UserId = currentUserId,
                };
            }

            int rowsAffected = await _applicationDbContext.Languages
                .Where(t => t.Id == language.Id && t.RowVersion == language.RowVersion)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.Name, language.Name)
                    .SetProperty(x => x.DisplayName, language.DisplayName)
                    .SetProperty(x => x.TwoLetterISOLanguageName, language.TwoLetterISOLanguageName)
                    .SetProperty(x => x.ThreeLetterISOLanguageName, language.ThreeLetterISOLanguageName)
                    .SetProperty(x => x.ThreeLetterWindowsLanguageName, language.ThreeLetterWindowsLanguageName)
                    .SetProperty(x => x.EnglishName, language.EnglishName)
                    .SetProperty(x => x.LastEditedBy, currentUserId), cancellationToken);

            if (rowsAffected == 0)
            {
                throw new EntityConcurrencyException()
                {
                    EntityName = nameof(Language),
                    EntityId = language.Id,
                };
            }

            return language;
        }

        public async Task DeleteLanguageAsync(int languageId, int currentUserId, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            var language = await _applicationDbContext.Languages
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == languageId, cancellationToken);

            if (language == null)
            {
                throw new EntityNotFoundException()
                {
                    EntityName = nameof(Language),
                    EntityId = languageId,
                };
            }

            bool isAuthorized = await _aclService.CheckUserObjectAsync(currentUserId, language, Actions.CanWrite, cancellationToken);

            if (!isAuthorized)
            {
                throw new EntityUnauthorizedAccessException()
                {
                    EntityName = nameof(Language),
                    EntityId = languageId,
                    UserId = currentUserId,
                };
            }

            await _applicationDbContext.Languages
                .Where(t => t.Id == language.Id)
                .ExecuteDeleteAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
