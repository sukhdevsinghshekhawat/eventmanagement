using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyMvcApp.Models;

namespace MyMvcApp.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventDepartment> EventDepartments { get; set; }
        public DbSet<EventApplication> EventApplications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<EventDepartment>()
                .HasKey(ed => new { ed.EventId, ed.Department });

            builder.Entity<EventDepartment>()
                .HasOne(ed => ed.Event)
                .WithMany(e => e.TargetDepartments)
                .HasForeignKey(ed => ed.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<EventApplication>()
                .HasOne(a => a.Event)
                .WithMany(e => e.Applications)
                .HasForeignKey(a => a.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<EventApplication>()
                .HasOne(a => a.Applicant)
                .WithMany()
                .HasForeignKey(a => a.ApplicantUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<EventApplication>()
                .HasIndex(a => new { a.EventId, a.ApplicantUserId })
                .IsUnique();
        }
    }
}