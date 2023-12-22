// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RebacExperiments.Server.OpenFga.Models;
namespace RebacExperiments.Server.OpenFga
{
    /// <summary>
    /// A <see cref="DbContext"/> to run raw queries on the OpenFGA Postgres Database.
    /// </summary>
    public class OpenFgaDbContext : DbContext
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="options">Options to configure the base <see cref="DbContext"/></param>
        public OpenFgaDbContext(DbContextOptions<OpenFgaDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the Assertions.
        /// </summary>
        public DbSet<FgaAssertion> Assertion { get; set; } = null!;

        /// <summary>
        /// Gets or sets the AuthorizationModels.
        /// </summary>
        public DbSet<FgaAuthorizationModel> AuthorizationModels { get; set; } = null!;

        /// <summary>
        /// Gets or sets the Changelogs.
        /// </summary>
        public DbSet<FgaChangelog> Changelogs{ get; set; } = null!;

        /// <summary>
        /// Gets or sets the Stores.
        /// </summary>
        public DbSet<FgaStore> Stores { get; set; } = null!;

        /// <summary>
        /// Gets or sets the Stores.
        /// </summary>
        public DbSet<FgaTuple> Tuples { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FgaAssertion>(entity =>
            {
                entity.ToTable("assertion", "public");

                entity.HasKey(e => new { e.Store, e.AuthorizationModelId });

                entity.Property(x => x.Store)
                    .HasColumnType("text")
                    .HasColumnName("store")
                    .IsRequired(true);

                entity.Property(e => e.AuthorizationModelId)
                    .HasColumnType("text")
                    .HasColumnName("authorization_model_id")
                    .IsRequired(true);

                entity.Property(e => e.Assertions)
                    .HasColumnType("bytea")
                    .HasColumnName("assertions");
            });

            modelBuilder.Entity<FgaAuthorizationModel>(entity =>
            {
                entity.ToTable("authorization_model", "public");

                entity.HasKey(e => new { e.Store, e.AuthorizationModelId, e.Type });

                entity.Property(x => x.Store)
                    .HasColumnType("text")
                    .HasColumnName("store")
                    .IsRequired(true);

                entity.Property(e => e.AuthorizationModelId)
                    .HasColumnType("text")
                    .HasColumnName("authorization_model_id")
                    .IsRequired(true);

                entity.Property(e => e.Type)
                    .HasColumnType("text")
                    .HasColumnName("type")
                    .IsRequired(true);

                entity.Property(e => e.TypeDefinition)
                    .HasColumnType("bytea")
                    .HasColumnName("type_definition");

                entity.Property(e => e.SchemaVersion)
                    .HasColumnType("text")
                    .HasColumnName("schema_version");

                entity.Property(e => e.SerializedProtobuf)
                    .HasColumnType("bytea")
                    .HasColumnName("serialized_protobuf");
            });
            
            modelBuilder.Entity<FgaChangelog>(entity =>
            {
                entity.ToTable("changelog", "public");

                entity.HasKey(e => new { e.Store, e.Ulid, e.ObjectType });

                entity.Property(x => x.Store)
                    .HasColumnType("text")
                    .HasColumnName("store")
                    .IsRequired(true);

                entity.Property(e => e.ObjectType)
                    .HasColumnType("text")
                    .HasColumnName("object_type")
                    .IsRequired(true);
                
                entity.Property(e => e.ObjectId)
                    .HasColumnType("text")
                    .HasColumnName("object_id")
                    .IsRequired(true);

                entity.Property(e => e.Relation)
                    .HasColumnType("text")
                    .HasColumnName("relation")
                    .IsRequired(true);
                
                entity.Property(e => e.User)
                    .HasColumnType("text")
                    .HasColumnName("_user")
                    .IsRequired(true);

                entity.Property(e => e.Operation)
                    .HasColumnType("integer")
                    .HasColumnName("operation")
                    .IsRequired(true);
                
                entity.Property(e => e.Ulid)
                    .HasColumnType("text")
                    .HasColumnName("ulid")
                    .IsRequired(true);
                
                entity.Property(e => e.InsertedAt)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("inserted_at")
                    .IsRequired(true);

                entity.Property(e => e.ConditionName)
                    .HasColumnType("text")
                    .HasColumnName("condition_name")
                    .IsRequired(true);

                entity.Property(e => e.ConditionContext)
                    .HasColumnType("bytea")
                    .HasColumnName("condition_context");
            });

            modelBuilder.Entity<FgaStore>(entity =>
            {
                entity.ToTable("store", "public");

                entity.HasKey(e => e.Id);

                entity.Property(x => x.Id)
                    .HasColumnType("text")
                    .HasColumnName("id")
                    .IsRequired(true);
                
                entity.Property(x => x.Name)
                    .HasColumnType("text")
                    .HasColumnName("name")
                    .IsRequired(true);

                entity.Property(e => e.CreatedAt)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("inserted_at")
                    .IsRequired(true);
                
                entity.Property(e => e.UpdatedAt)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("updated_at")
                    .IsRequired(false);
                
                entity.Property(e => e.DeletedAt)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("deleted_at")
                    .IsRequired(false);
            });

            modelBuilder.Entity<FgaTuple>(entity =>
            {
                entity.ToTable("tuple", "public");

                entity.HasKey(e => new { e.Store, e.Ulid, e.ObjectType });

                entity.Property(x => x.Store)
                    .HasColumnType("text")
                    .HasColumnName("store")
                    .IsRequired(true);

                entity.Property(e => e.ObjectType)
                    .HasColumnType("text")
                    .HasColumnName("object_type")
                    .IsRequired(true);

                entity.Property(e => e.ObjectId)
                    .HasColumnType("text")
                    .HasColumnName("object_id")
                    .IsRequired(true);

                entity.Property(e => e.Relation)
                    .HasColumnType("text")
                    .HasColumnName("relation")
                    .IsRequired(true);

                entity.Property(e => e.User)
                    .HasColumnType("text")
                    .HasColumnName("_user")
                    .IsRequired(true);

                entity.Property(e => e.UserType)
                    .HasColumnType("text")
                    .HasColumnName("user_type")
                    .IsRequired(true);

                entity.Property(e => e.Ulid)
                    .HasColumnType("text")
                    .HasColumnName("ulid")
                    .IsRequired(true);

                entity.Property(e => e.InsertedAt)
                    .HasColumnType("timestamp with time zone")
                    .HasColumnName("inserted_at")
                    .IsRequired(true);

                entity.Property(e => e.ConditionName)
                    .HasColumnType("text")
                    .HasColumnName("condition_name")
                    .IsRequired(true);

                entity.Property(e => e.ConditionContext)
                    .HasColumnType("bytea")
                    .HasColumnName("condition_context");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
