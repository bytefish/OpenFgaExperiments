namespace RebacExperiments.Server.Api.Infrastructure.Authorization
{
    public class StoredRelationTuple
    {
        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public required string Id { get; set; }

        /// <summary>
        /// Gets or sets the Store.
        /// </summary>
        public required string Store { get; set; }

        /// <summary>
        /// Gets or sets the Object.
        /// </summary>
        public required string Object { get; set; }

        /// <summary>
        /// Gets or sets the Relation.
        /// </summary>
        public required string Relation { get; set; }

        /// <summary>
        /// Gets or sets the Subject.
        /// </summary>
        public required string Subject { get; set; }

        /// <summary>
        /// Gets or sets the InsertedAt.
        /// </summary>
        public required DateTime InsertedAt { get; set; }
    }
}
