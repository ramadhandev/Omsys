using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OMSys.Models;

namespace OMSys.Data
{
    // Ubah dari DbContext → IdentityDbContext
    public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Unit> Units { get; set; }
        public DbSet<Component> Components { get; set; }
        public DbSet<Symptom> Symptoms { get; set; }
        public DbSet<DiagnosisStep> DiagnosisSteps { get; set; }
        public DbSet<StepResult> StepResults { get; set; }
        public DbSet<Solution> Solutions { get; set; }
        public DbSet<TroubleshootingView> TroubleshootingViews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // 👈 WAJIB panggil ini agar Identity tables dibuat

            // StepResult → Step (required)
            modelBuilder.Entity<StepResult>()
                .HasOne(sr => sr.Step)
                .WithMany(ds => ds.StepResults)
                .HasForeignKey(sr => sr.StepId)
                .OnDelete(DeleteBehavior.Restrict);

            // StepResult → NextStep (optional)
            modelBuilder.Entity<StepResult>()
                .HasOne(sr => sr.NextStep)
                .WithMany()
                .HasForeignKey(sr => sr.NextStepId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            // StepResult → Solution (optional)
            modelBuilder.Entity<StepResult>()
                .HasOne(sr => sr.Solution)
                .WithMany(s => s.StepResults)
                .HasForeignKey(sr => sr.SolutionId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            // Component → Unit
            modelBuilder.Entity<Component>()
                .HasOne(c => c.Unit)
                .WithMany(u => u.Components)
                .HasForeignKey(c => c.UnitId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            // Symptom → Component
            modelBuilder.Entity<Symptom>()
                .HasOne(s => s.Component)
                .WithMany(c => c.Symptoms)
                .HasForeignKey(s => s.ComponentId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            // DiagnosisStep → Symptom
            modelBuilder.Entity<DiagnosisStep>()
                .HasOne(ds => ds.Symptom)
                .WithMany(s => s.DiagnosisSteps)
                .HasForeignKey(ds => ds.SymptomId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            // Solution → Symptom
            modelBuilder.Entity<Solution>()
                .HasOne(s => s.Symptom)
                .WithMany(s => s.Solutions)
                .HasForeignKey(s => s.SymptomId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            // Ignore non-entity view models
            modelBuilder.Entity<TroubleshootingView>().HasNoKey();
            modelBuilder.Entity<StepView>().HasNoKey();
        }
    }
}
