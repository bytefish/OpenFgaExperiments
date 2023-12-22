using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RebacExperiments.Server.OpenFga.Models
{
    public class FgaStore
    {
        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public required string Id { get; set; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Gets or sets the CreatedAt.
        /// </summary>
        public required DateTime? CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the UpdatedAt.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the DeletedAt.
        /// </summary>
        public DateTime? DeletedAt { get; set; }
    }
}
