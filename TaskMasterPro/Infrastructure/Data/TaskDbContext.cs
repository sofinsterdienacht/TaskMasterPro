using Microsoft.EntityFrameworkCore;
using TaskMasterPro.Core.Entities;

namespace TaskMasterPro.Infrastructure.Data;

public class TaskDbContext : DbContext
{
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<PriorityLookup> Priorities { get; set; }
    public DbSet<StatusLookup> Statuses { get; set; }
    public DbSet<CategoryLookup> Categories { get; set; }

    public TaskDbContext(DbContextOptions<TaskDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Конфигурация для TaskItem
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.PriorityId).IsRequired();
            entity.Property(e => e.StatusId).IsRequired();
            entity.Property(e => e.CategoryId).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();

            // Связи со справочниками
            entity.HasOne(e => e.PriorityRef)
                .WithMany()
                .HasForeignKey(e => e.PriorityId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.StatusRef)
                .WithMany()
                .HasForeignKey(e => e.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.CategoryRef)
                .WithMany()
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Конфигурация справочников
        modelBuilder.Entity<PriorityLookup>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(64);
            entity.ToTable("Priorities");
            entity.HasData(
                new PriorityLookup { Id = (int)TaskPriority.Low, Name = nameof(TaskPriority.Low) },
                new PriorityLookup { Id = (int)TaskPriority.Medium, Name = nameof(TaskPriority.Medium) },
                new PriorityLookup { Id = (int)TaskPriority.High, Name = nameof(TaskPriority.High) },
                new PriorityLookup { Id = (int)TaskPriority.Urgent, Name = nameof(TaskPriority.Urgent) }
            );
        });

        modelBuilder.Entity<StatusLookup>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(64);
            entity.ToTable("Statuses");
            entity.HasData(
                new StatusLookup { Id = (int)TaskItemStatus.Pending, Name = nameof(TaskItemStatus.Pending) },
                new StatusLookup { Id = (int)TaskItemStatus.InProgress, Name = nameof(TaskItemStatus.InProgress) },
                new StatusLookup { Id = (int)TaskItemStatus.Completed, Name = nameof(TaskItemStatus.Completed) },
                new StatusLookup { Id = (int)TaskItemStatus.Cancelled, Name = nameof(TaskItemStatus.Cancelled) }
            );
        });

        modelBuilder.Entity<CategoryLookup>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(64);
            entity.ToTable("Categories");
            entity.HasData(
                new CategoryLookup { Id = (int)TaskCategory.Personal, Name = nameof(TaskCategory.Personal) },
                new CategoryLookup { Id = (int)TaskCategory.Work, Name = nameof(TaskCategory.Work) },
                new CategoryLookup { Id = (int)TaskCategory.Study, Name = nameof(TaskCategory.Study) },
                new CategoryLookup { Id = (int)TaskCategory.Health, Name = nameof(TaskCategory.Health) },
                new CategoryLookup { Id = (int)TaskCategory.Finance, Name = nameof(TaskCategory.Finance) },
                new CategoryLookup { Id = (int)TaskCategory.Shopping, Name = nameof(TaskCategory.Shopping) }
            );
        });
    }
}