using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PraceticeAssesment.Entity.Models;

namespace PraceticeAssesment.Entity;

public class DatabaseContext : IdentityDbContext<UserEntity, IdentityRole<int>, int>
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }


    public virtual DbSet<TaskEntity> Tasks { get; set; }

    public virtual DbSet<RelUserTaskEntity> RelUserTasks { get; set; }

    public virtual DbSet<TaskHistory> TaskHistories { get; set; }

    public virtual DbSet<TaskCommentEntity> TaskComments { get; set; }

    public virtual DbSet<NotificationEntity> NotificationEntities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.ToTable("Users"); // Map UserEntity to Users table

            entity.HasKey(entity => entity.Id); // Set Id as primary key

            entity.HasMany(e => e.RelUserTaskEntities)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<IdentityRole<int>>(entity => {
            entity.ToTable(name: "AspNetRoles");
            entity.HasKey(e => e.Id);
        });

        // Define primary keys for Identity-related entities
        modelBuilder.Entity<IdentityUserLogin<int>>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });
        });

        modelBuilder.Entity<IdentityUserRole<int>>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId });
        });

        modelBuilder.Entity<IdentityUserToken<int>>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });
        });

        modelBuilder.Entity<IdentityUserClaim<int>>(entity =>
        {
            entity.HasKey(e => new { e.Id });
        });

        modelBuilder.Entity<IdentityRoleClaim<int>>(entity => {
            
            entity.HasKey(e => e.Id);
        });

        modelBuilder.Entity<TaskEntity>(entity =>
        {
            entity.ToTable("Tasks"); // Map TaskEntity to Tasks table

            entity.HasMany(e => e.TaskHistories)
                .WithOne(e => e.Task)
                .HasForeignKey(e => e.TaskId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasMany(e => e.RelUserTaskEntities)
                .WithOne(e => e.Task)
                .HasForeignKey(e => e.TaskId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasMany(e => e.TaskComments)
                .WithOne(e => e.Task)
                .HasForeignKey(e => e.TaskId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<RelUserTaskEntity>(entity => 
        {
            entity.ToTable("RelUserTask");

            entity.HasOne(e => e.User)
                .WithMany(e => e.RelUserTaskEntities)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(e => e.Task)
                .WithMany(e => e.RelUserTaskEntities)
                .HasForeignKey(e => e.TaskId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<TaskHistory>(entity => 
        {
            entity.ToTable("TaskHistory");

            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Task)
                .WithMany(e => e.TaskHistories)
                .HasForeignKey(e => e.TaskId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<TaskCommentEntity>(entity =>
        {
            entity.ToTable("TaskComments");

            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Task)
                .WithMany(e => e.TaskComments)
                .HasForeignKey(e => e.TaskId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(e => e.User)
                .WithMany(e => e.TaskCommentEntities)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<NotificationEntity>(entity =>
        {
            entity.ToTable("Notifications");

            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Task)
                .WithMany(e => e.NotificationEntities)
                .HasForeignKey(e => e.TaskId)
                .OnDelete(DeleteBehavior.NoAction);
        });
    }
}
