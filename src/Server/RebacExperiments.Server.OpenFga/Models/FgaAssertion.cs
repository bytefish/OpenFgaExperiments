using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RebacExperiments.Server.OpenFga.Models
{
    public class FgaAssertion
    {
        /// <summary>
        /// Gets or sets the Store.
        /// </summary>
        public required string Store { get; set; }

        /// <summary>
        /// Gets or sets the AuthorizationModelId.
        /// </summary>
        public required string AuthorizationModelId { get; set; }

        /// <summary>
        /// Gets or sets the Assertions.
        /// </summary>
        public byte[]? Assertions { get; set; }
    }
}
