using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UniNotes.Models;

namespace UniNotes.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Note> Notes => Set<Note>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Tag> Tags => Set<Tag>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Note>()
            .HasOne(n => n.Course)
            .WithMany(c => c.Notes)
            .HasForeignKey(n => n.CourseId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Note>()
            .HasMany(n => n.Tags)
            .WithMany(t => t.Notes);
    }
}