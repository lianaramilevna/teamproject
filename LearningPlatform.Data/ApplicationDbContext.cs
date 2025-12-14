using LearningPlatform.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CourseMembership> CourseMemberships => Set<CourseMembership>();
    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<Submission> Submissions => Set<Submission>();
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<TeamMembership> TeamMemberships => Set<TeamMembership>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.Email).IsUnique();
            entity.Property(x => x.Email).IsRequired().HasMaxLength(256);
            entity.Property(x => x.PasswordHash).IsRequired();
            entity.Property(x => x.Role).IsRequired();
            entity.Property(x => x.CreatedAtUtc).IsRequired();
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Title).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Description).HasMaxLength(2000);
            entity.Property(x => x.JoinCode).IsRequired().HasMaxLength(12);
            entity.HasIndex(x => x.JoinCode).IsUnique();
            entity.Property(x => x.InstructorId).IsRequired();
            entity.Property(x => x.CreatedAtUtc).IsRequired();
        });

        modelBuilder.Entity<CourseMembership>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.CourseId, x.UserId }).IsUnique();
            entity.Property(x => x.JoinedAtUtc).IsRequired();
            entity.HasOne<Course>()
                .WithMany()
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Title).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Description).HasMaxLength(4000);
            entity.Property(x => x.DueDateUtc).IsRequired(false);
            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.HasOne<Course>()
                .WithMany()
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Team>()
                .WithMany()
                .HasForeignKey(x => x.TeamId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);
        });

        modelBuilder.Entity<Submission>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Link).IsRequired().HasMaxLength(2000);
            entity.Property(x => x.SubmittedAtUtc).IsRequired();
            entity.Property(x => x.Grade).IsRequired(false);
            entity.Property(x => x.Comment).HasMaxLength(2000);
            entity.HasOne<Assignment>()
                .WithMany()
                .HasForeignKey(x => x.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(200);
            entity.Property(x => x.CourseId).IsRequired();
            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.HasOne<Course>()
                .WithMany()
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<TeamMembership>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => new { x.TeamId, x.UserId }).IsUnique();
            entity.Property(x => x.JoinedAtUtc).IsRequired();
            entity.HasOne<Team>()
                .WithMany()
                .HasForeignKey(x => x.TeamId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
