// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.RateLimiting;
using RebacExperiments.Server.Api.Infrastructure.Authentication;
using RebacExperiments.Server.Api.Infrastructure.Constants;
using RebacExperiments.Server.Api.Infrastructure.Errors;
using RebacExperiments.Server.Api.Infrastructure.Exceptions;
using RebacExperiments.Server.Api.Infrastructure.Logging;
using RebacExperiments.Server.Api.Services;
using RebacExperiments.Server.Database.Models;

namespace RebacExperiments.Server.Api.Controllers
{
    public class LanguagesController : ODataController
    {
        private readonly ILogger<UsersController> _logger;

        private readonly ExceptionToODataErrorMapper _exceptionToODataErrorMapper;

        public LanguagesController(ILogger<UsersController> logger, ExceptionToODataErrorMapper exceptionToODataErrorMapper)
        {
            _logger = logger;
            _exceptionToODataErrorMapper = exceptionToODataErrorMapper;
        }

        [Authorize(Policy = Policies.RequireUserRole)]
        [EnableRateLimiting(Policies.PerUserRatelimit)]
        public async Task<IActionResult> GetLanguage([FromServices] ILanguageService languagesService, [FromODataUri(Name = "key")] int key, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            try
            {
                if (!ModelState.IsValid)
                {
                    throw new InvalidModelStateException
                    {
                        ModelStateDictionary = ModelState
                    };
                }

                var user = await languagesService.GetLanguageByIdAsync(key, User.GetUserId(), cancellationToken);

                return Ok(user);
            }
            catch (Exception exception)
            {
                return _exceptionToODataErrorMapper.CreateODataErrorResult(HttpContext, exception);
            }
        }

        [HttpGet]
        [Authorize(Policy = Policies.RequireUserRole)]
        [EnableRateLimiting(Policies.PerUserRatelimit)]
        public async Task<IActionResult> GetLanguages([FromServices] ILanguageService languageService, ODataQueryOptions<Language> queryOptions, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            try
            {
                if (!ModelState.IsValid)
                {
                    throw new InvalidModelStateException
                    {
                        ModelStateDictionary = ModelState
                    };
                }

                var languages = await languageService.GetAllLanguagesAsync(cancellationToken);

               return Ok(queryOptions.ApplyTo(languages.AsQueryable()));
            }
            catch (Exception exception)
            {
                return _exceptionToODataErrorMapper.CreateODataErrorResult(HttpContext, exception);
            }
        }

        [HttpPost]
        [Authorize(Policy = Policies.RequireUserRole)]
        [EnableRateLimiting(Policies.PerUserRatelimit)]
        public async Task<IActionResult> PostLanguage([FromServices] ILanguageService languageService, [FromBody] Language language, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            try
            {
                if (!ModelState.IsValid)
                {
                    throw new InvalidModelStateException
                    {
                        ModelStateDictionary = ModelState
                    };
                }

                await languageService.CreateLanguageAsync(language, User.GetUserId(), cancellationToken);

                return Created(language);
            }
            catch (Exception exception)
            {
                return _exceptionToODataErrorMapper.CreateODataErrorResult(HttpContext, exception);
            }
        }

        [HttpPut]
        [HttpPatch]
        [Authorize(Policy = Policies.RequireUserRole)]
        [EnableRateLimiting(Policies.PerUserRatelimit)]
        public async Task<IActionResult> PatchLanguage([FromServices] ILanguageService languageService, [FromODataUri] int key, [FromBody] Delta<Language> delta, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            try
            {
                if (!ModelState.IsValid)
                {
                    throw new InvalidModelStateException
                    {
                        ModelStateDictionary = ModelState
                    };
                }

                var language = await languageService.GetLanguageByIdAsync(key, User.GetUserId(), cancellationToken);

                // Patch the Values to it:
                delta.Patch(language);

                // Update the Values:
                await languageService.UpdateLanguageAsync(language, User.GetUserId(), cancellationToken);

                return Updated(language);
            }
            catch (Exception exception)
            {
                return _exceptionToODataErrorMapper.CreateODataErrorResult(HttpContext, exception);
            }
        }

        [HttpDelete]
        [Authorize(Policy = Policies.RequireUserRole)]
        [EnableRateLimiting(Policies.PerUserRatelimit)]
        public async Task<IActionResult> DeleteLanguage([FromServices] ILanguageService languageService, [FromODataUri(Name = "key")] int key, CancellationToken cancellationToken)
        {
            _logger.TraceMethodEntry();

            try
            {
                if (!ModelState.IsValid)
                {
                    throw new InvalidModelStateException
                    {
                        ModelStateDictionary = ModelState
                    };
                }

                await languageService.DeleteLanguageAsync(key, User.GetUserId(), cancellationToken);

                return StatusCode(StatusCodes.Status204NoContent);
            }
            catch (Exception exception)
            {
                return _exceptionToODataErrorMapper.CreateODataErrorResult(HttpContext, exception);
            }
        }
    }
}