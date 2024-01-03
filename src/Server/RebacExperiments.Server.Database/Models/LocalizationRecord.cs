// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace RebacExperiments.Server.Database.Models
{
    public class LocalizationRecord : Entity
    {
        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Gets or sets the Category.
        /// </summary>
        public required string Category { get; set; }

        /// <summary>
        /// Gets or sets the Value.
        /// </summary>
        public required string Value { get; set; }

        /// <summary>
        /// Gets or sets the LanguageId.
        /// </summary>
        public required int LanguageId { get; set; }
    }
}