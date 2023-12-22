namespace RebacExperiments.Server.OpenFga.Models
{
    public class FgaAuthorizationModel
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
        /// Gets or sets the Type.
        /// </summary>
        public required string Type { get; set; }

        /// <summary>
        /// Gets or sets the TypeDefinition.
        /// </summary>
        public byte[]? TypeDefinition { get; set; }

        /// <summary>
        /// Gets or sets the SchemaVersion.
        /// </summary>
        public required string SchemaVersion { get; set; }

        /// <summary>
        /// Gets or sets the SerializedProtobuf.
        /// </summary>
        public byte[]? SerializedProtobuf { get; set; }
    }
}
