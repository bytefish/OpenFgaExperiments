// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace RebacExperiments.Server.Database.Models
{
    public class Language : Entity
    {
        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Gets or sets the DisplayName.
        /// </summary>
        public required string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the EnglishName.
        /// </summary>
        public required string EnglishName { get; set; }

        /// <summary>
        /// Gets or sets the TwoLetterISOLanguageName.
        /// </summary>
        public required string TwoLetterISOLanguageName { get; set; }

        /// <summary>
        /// Gets or sets the ThreeLetterISOLanguageName.
        /// </summary>
        public required string ThreeLetterISOLanguageName { get; set; }

        /// <summary>
        /// Gets or sets the ThreeLetterWindowsLanguageName.
        /// </summary>
        public required string ThreeLetterWindowsLanguageName { get; set; }
    }
}