using Formit.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Formit.Infraestructure.Data.Contexts;
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            if (Environment.GetEnvironmentVariable("DBCONN") != null)
                optionsBuilder.UseNpgsql(Environment.GetEnvironmentVariable("DBCONN"));
        }
    }

    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<QuestionOption> QuestionOptions { get; set; }
    public DbSet<FormSubmission> FormSubmissions { get; set; }
    public DbSet<QuestionResponse> QuestionResponses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<QuestionResponse>()
            .HasIndex(qr => new { qr.FormSubmissionId, qr.QuestionId })
            .IsUnique();

        modelBuilder.Entity<QuestionResponse>()
            .HasOne(qr => qr.ChosenOption)
            .WithMany()
            .HasForeignKey(qr => qr.ChosenOptionId)
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);
    }
}
