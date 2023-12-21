// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using RebacExperiments.Server.Api.Models;

namespace RebacExperiments.Server.Api.Infrastructure.Database
{
    /// <summary>
    /// A <see cref="DbContext"/> to query the database.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Logger.
        /// </summary>
        internal ILogger<ApplicationDbContext> Logger { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="options">Options to configure the base <see cref="DbContext"/></param>
        public ApplicationDbContext(ILogger<ApplicationDbContext> logger, DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Logger = logger;
        }

        /// <summary>
        /// Gets or sets the Users.
        /// </summary>
        public DbSet<User> Users { get; set; } = null!;

        /// <summary>
        /// Gets or sets the Roles.
        /// </summary>
        public DbSet<Role> Roles { get; set; } = null!;

        /// <summary>
        /// Gets or sets the TaskItems.
        /// </summary>
        public DbSet<TaskItem> TaskItems { get; set; } = null!;

        /// <summary>
        /// Gets or sets the TaskItems.
        /// </summary>
        public DbSet<Team> Teams { get; set; } = null!;

        /// <summary>
        /// Gets or sets the TaskItems.
        /// </summary>
        public DbSet<Organization> Organizations { get; set; } = null!;

        /// <summary>
        /// Gets or sets the UserRoles.
        /// </summary>
        public DbSet<UserRole> UserRoles { get; set; } = null!;

        /// <summary>
        /// Gets or sets the UserRoles.
        /// </summary>
        public DbSet<UserTaskItem> UserTaskItems { get; set; } = null!;

        /// <summary>
        /// Gets or sets the UserTeams.
        /// </summary>
        public DbSet<TeamRole> TeamRoles { get; set; } = null!;

        /// <summary>
        /// Gets or sets the UserTeams.
        /// </summary>
        public DbSet<OrganizationRole> OrganizationRoles { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Now create the Tables
            modelBuilder.HasSequence<int>("sq_TaskItem", schema: "Application")
                .StartsAt(38187)
                .IncrementsBy(1);

            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.ToTable("TaskItem", "Application");

                entity.HasKey(e => e.Id);

                entity.Property(x => x.Id)
                    .HasColumnType("INT")
                    .HasColumnName("TaskItemID")
                    .UseHiLo("sq_TaskItem", "Application")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Title)
                    .HasColumnType("NVARCHAR(50)")
                    .HasColumnName("Title")
                    .IsRequired(true)
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .HasColumnType("NVARCHAR(2000)")
                    .HasColumnName("Description")
                    .IsRequired(true)
                    .HasMaxLength(2000);

                entity.Property(e => e.DueDateTime)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("DueDateTime")
                    .IsRequired(false);

                entity.Property(e => e.ReminderDateTime)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("ReminderDateTime")
                    .IsRequired(false);

                entity.Property(e => e.CompletedDateTime)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("CompletedDateTime")
                    .IsRequired(false);

                entity.Property(e => e.TaskItemStatus)
                    .HasColumnType("INT")
                    .HasColumnName("TaskItemStatusID")
                    .HasConversion(v => (int)v, v => (TaskItemStatusEnum)v)
                    .IsRequired(true);

                entity.Property(e => e.TaskItemPriority)
                    .HasColumnType("INT")
                    .HasColumnName("TaskItemPriorityID")
                    .HasConversion(v => (int)v, v => (TaskItemPriorityEnum)v)
                    .IsRequired(true);

                entity.Property(e => e.AssignedTo)
                    .HasColumnType("INT")
                    .HasColumnName("AssignedTo")
                    .IsRequired(false);

                entity.Property(e => e.RowVersion)
                    .HasColumnType("ROWVERSION")
                    .HasColumnName("RowVersion")
                    .IsConcurrencyToken()
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ValidFrom)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("ValidFrom")
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ValidTo)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("ValidTo")
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.LastEditedBy)
                    .HasColumnType("INT")
                    .HasColumnName("LastEditedBy")
                    .IsRequired(true);
            });

            modelBuilder.HasSequence<int>("sq_Organization", schema: "Application")
                .StartsAt(38187)
                .IncrementsBy(1);

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.ToTable("Organization", "Application");

                entity.HasKey(e => e.Id);

                entity.Property(x => x.Id)
                    .HasColumnType("INT")
                    .HasColumnName("OrganizationID")
                    .UseHiLo("sq_Organization", "Application")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .HasColumnType("NVARCHAR(255)")
                    .HasColumnName("Name")
                    .IsRequired(true)
                    .HasMaxLength(255);

                entity.Property(e => e.Description)
                    .HasColumnType("NVARCHAR(2000)")
                    .HasColumnName("Description")
                    .IsRequired(true)
                    .HasMaxLength(2000);

                entity.Property(e => e.RowVersion)
                    .HasColumnType("ROWVERSION")
                    .HasColumnName("RowVersion")
                    .IsConcurrencyToken()
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ValidFrom)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("ValidFrom")
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ValidTo)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("ValidTo")
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.LastEditedBy)
                    .HasColumnType("INT")
                    .HasColumnName("LastEditedBy")
                    .IsRequired(true);
            });

            modelBuilder.HasSequence<int>("sq_OrganizationRole", schema: "Application")
                .StartsAt(38187)
                .IncrementsBy(1);

            modelBuilder.Entity<OrganizationRole>(entity =>
            {
                entity.ToTable("OrganizationRole", "Application");

                entity.HasKey(e => e.Id);

                entity.Property(x => x.Id)
                    .HasColumnType("INT")
                    .HasColumnName("OrganizationRoleID")
                    .UseHiLo("sq_OrganizationRole", "Application")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.OrganizationId)
                    .HasColumnType("INT")
                    .HasColumnName("OrganizationID")
                    .IsRequired(true);

                entity.Property(e => e.UserId)
                    .HasColumnType("INT")
                    .HasColumnName("UserID")
                    .IsRequired(true);

                entity.Property(e => e.Role)
                    .HasColumnType("NVARCHAR(255)")
                    .HasColumnName("Name")
                    .IsRequired(true)
                    .HasMaxLength(255);

                entity.Property(e => e.RowVersion)
                    .HasColumnType("ROWVERSION")
                    .HasColumnName("RowVersion")
                    .IsConcurrencyToken()
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ValidFrom)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("ValidFrom")
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ValidTo)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("ValidTo")
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.LastEditedBy)
                    .HasColumnType("INT")
                    .HasColumnName("LastEditedBy")
                    .IsRequired(true);
            });

            modelBuilder.HasSequence<int>("sq_Team", schema: "Application")
                .StartsAt(38187)
                .IncrementsBy(1);

            modelBuilder.Entity<Team>(entity =>
            {
                entity.ToTable("Team", "Application");

                entity.HasKey(e => e.Id);

                entity.Property(x => x.Id)
                    .HasColumnType("INT")
                    .HasColumnName("TeamID")
                    .UseHiLo("sq_Team", "Application")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .HasColumnType("NVARCHAR(255)")
                    .HasColumnName("Name")
                    .IsRequired(true)
                    .HasMaxLength(255);

                entity.Property(e => e.Description)
                    .HasColumnType("NVARCHAR(2000)")
                    .HasColumnName("Description")
                    .IsRequired(true)
                    .HasMaxLength(2000);

                entity.Property(e => e.RowVersion)
                    .HasColumnType("ROWVERSION")
                    .HasColumnName("RowVersion")
                    .IsConcurrencyToken()
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ValidFrom)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("ValidFrom")
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ValidTo)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("ValidTo")
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.LastEditedBy)
                    .HasColumnType("INT")
                    .HasColumnName("LastEditedBy")
                    .IsRequired(true);
            });

            modelBuilder.HasSequence<int>("sq_TeamRole", schema: "Application")
                .StartsAt(38187)
                .IncrementsBy(1);

            modelBuilder.Entity<TeamRole>(entity =>
            {
                entity.ToTable("TeamRole", "Application");

                entity.HasKey(e => e.Id);

                entity.Property(x => x.Id)
                    .HasColumnType("INT")
                    .HasColumnName("TeamRoleID")
                    .UseHiLo("sq_TeamRole", "Application")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.TeamId)
                    .HasColumnType("INT")
                    .HasColumnName("TeamID")
                    .IsRequired(true);

                entity.Property(e => e.UserId)
                    .HasColumnType("INT")
                    .HasColumnName("UserID")
                    .IsRequired(true);

                entity.Property(e => e.Role)
                    .HasColumnType("NVARCHAR(255)")
                    .HasColumnName("Name")
                    .IsRequired(true)
                    .HasMaxLength(255);

                entity.Property(e => e.RowVersion)
                    .HasColumnType("ROWVERSION")
                    .HasColumnName("RowVersion")
                    .IsConcurrencyToken()
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ValidFrom)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("ValidFrom")
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ValidTo)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("ValidTo")
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.LastEditedBy)
                    .HasColumnType("INT")
                    .HasColumnName("LastEditedBy")
                    .IsRequired(true);
            });

            modelBuilder.HasSequence<int>("sq_User", schema: "Identity")
                .StartsAt(38187)
                .IncrementsBy(1);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User", "Identity");

                entity.HasKey(e => e.Id);

                entity.Property(x => x.Id)
                    .HasColumnType("INT")
                    .HasColumnName("UserID")
                    .UseHiLo("sq_User", "Identity")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.FullName)
                    .HasColumnType("NVARCHAR(50)")
                    .HasColumnName("FullName")
                    .IsRequired(true)
                    .HasMaxLength(50);

                entity.Property(e => e.PreferredName)
                    .HasColumnType("NVARCHAR(50)")
                    .HasColumnName("PreferredName")
                    .IsRequired(true)
                    .HasMaxLength(50);

                entity.Property(e => e.IsPermittedToLogon)
                    .HasColumnType("BIT")
                    .HasColumnName("IsPermittedToLogon")
                    .IsRequired(true)
                    .HasMaxLength(50);

                entity.Property(e => e.LogonName)
                    .HasColumnType("NVARCHAR(256)")
                    .HasColumnName("LogonName")
                    .IsRequired(false)
                    .HasMaxLength(50);

                entity.Property(e => e.HashedPassword)
                    .HasColumnType("NVARCHAR(MAX)")
                    .HasColumnName("HashedPassword")
                    .IsRequired(false);

                entity.Property(e => e.RowVersion)
                    .HasColumnType("ROWVERSION")
                    .HasColumnName("RowVersion")
                    .IsRequired(false)
                    .IsConcurrencyToken()
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ValidFrom)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("ValidFrom")
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ValidTo)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("ValidTo")
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.LastEditedBy)
                    .HasColumnType("INT")
                    .HasColumnName("LastEditedBy")
                    .IsRequired(true);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Role", "Identity");

                entity.HasKey(e => e.Id);

                entity.Property(x => x.Id)
                    .HasColumnType("INT")
                    .HasColumnName("RoleID")
                    .UseHiLo("[Application].[sq_Role]")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .HasColumnType("NVARCHAR(255)")
                    .HasColumnName("Name")
                    .IsRequired(true)
                    .HasMaxLength(255);

                entity.Property(e => e.Description)
                    .HasColumnType("NVARCHAR(2000)")
                    .HasColumnName("Description")
                    .IsRequired(true)
                    .HasMaxLength(2000);

                entity.Property(e => e.RowVersion)
                    .HasColumnType("ROWVERSION")
                    .HasColumnName("RowVersion")
                    .IsConcurrencyToken()
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ValidFrom)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("ValidFrom")
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ValidTo)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("ValidTo")
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.LastEditedBy)
                    .HasColumnType("INT")
                    .HasColumnName("LastEditedBy")
                    .IsRequired(true);
            });


            modelBuilder.HasSequence<int>("sq_UserTaskItem", schema: "Application")
                .StartsAt(38187)
                .IncrementsBy(1);

            modelBuilder.Entity<UserTaskItem>(entity =>
            {
                entity.ToTable("UserTaskItem", "Application");

                entity.HasKey(e => e.Id);

                entity.Property(x => x.Id)
                    .HasColumnType("INT")
                    .HasColumnName("UserTaskItemID")
                    .UseHiLo("sq_UserTaskItem", "Application")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.TaskItemId)
                    .HasColumnType("INT")
                    .HasColumnName("TaskItemID")
                    .IsRequired(true);

                entity.Property(e => e.UserId)
                    .HasColumnType("INT")
                    .HasColumnName("UserID")
                    .IsRequired(true);

                entity.Property(e => e.Role)
                    .HasColumnType("NVARCHAR(255)")
                    .HasColumnName("Name")
                    .IsRequired(true)
                    .HasMaxLength(255);

                entity.Property(e => e.RowVersion)
                    .HasColumnType("ROWVERSION")
                    .HasColumnName("RowVersion")
                    .IsConcurrencyToken()
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ValidFrom)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("ValidFrom")
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.ValidTo)
                    .HasColumnType("DATETIME2(7)")
                    .HasColumnName("ValidTo")
                    .IsRequired(false)
                    .ValueGeneratedOnAddOrUpdate();

                entity.Property(e => e.LastEditedBy)
                    .HasColumnType("INT")
                    .HasColumnName("LastEditedBy")
                    .IsRequired(true);
            });



            base.OnModelCreating(modelBuilder);
        }
    }
}
