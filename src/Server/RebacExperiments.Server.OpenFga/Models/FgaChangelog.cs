using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;
using static System.Net.Mime.MediaTypeNames;

namespace RebacExperiments.Server.OpenFga.Models
{
    public class FgaChangelog
    {
        /// <summary>
        /// Gets or sets the Store.
        /// </summary>
        public required string Store { get; set; }

        /// <summary>
        /// Gets or sets the ObjectType.
        /// </summary>
        public required string ObjectType { get; set; }

        /// <summary>
        /// Gets or sets the ObjectId.
        /// </summary>
        public required string ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the Relation.
        /// </summary>
        public required string Relation { get; set; }

        /// <summary>
        /// Gets or sets the User.
        /// </summary>
        public required string User { get; set; }

        /// <summary>
        /// Gets or sets the Operation.
        /// </summary>
        public required int Operation { get; set; }

        /// <summary>
        /// Gets or sets the Ulid.
        /// </summary>
        public required string Ulid { get; set; }

        /// <summary>
        /// Gets or sets the InsertedAt.
        /// </summary>
        public required DateTime InsertedAt { get; set; }

        /// <summary>
        /// Gets or sets the ConditionName.
        /// </summary>
        public string? ConditionName { get; set; }

        /// <summary>
        /// Gets or sets the ConditionContext.
        /// </summary>
        public byte[]? ConditionContext { get; set; }

        //CONSTRAINT changelog_pkey PRIMARY KEY (store, ulid, object_type)
    }
}
