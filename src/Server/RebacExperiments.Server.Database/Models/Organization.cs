﻿// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace RebacExperiments.Server.Database.Models
{
    public class Organization : Entity
    {
        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        public required string Description { get; set; }
    }
}
