namespace RebacExperiments.Server.Api.Infrastructure.Authorization
{
    public record RelationTuple
    {
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
    }
}
