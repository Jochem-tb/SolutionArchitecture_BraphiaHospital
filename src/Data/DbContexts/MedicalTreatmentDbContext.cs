using Data.Models.MedicalTreatment;
using Microsoft.EntityFrameworkCore;

namespace Data.DbContexts;

public class MedicalTreatmentDbContext : DbContext
{
    public MedicalTreatmentDbContext()
    {
    }

    public MedicalTreatmentDbContext(DbContextOptions<MedicalTreatmentDbContext> options) : base(options)
    {
    }

    public DbSet<MedicalTreatment> MedicalTreatments { get; set; }
    public DbSet<MedicalHistoryEntry> MedicalHistoryEntries { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MedicalTreatment>(entity =>
        {
            entity.ToTable("MedicalTreatments");
            entity.HasKey(t => t.Id);
            entity.Property(t => t.Type).HasMaxLength(100);
            entity.Property(t => t.Results).HasColumnType("nvarchar(max)");
            entity.HasIndex(t => t.PatientId);
            entity.HasIndex(t => t.PhysicianId);
            entity.HasIndex(t => t.AppointmentId).IsUnique().HasFilter("[AppointmentId] IS NOT NULL");
        });

        modelBuilder.Entity<MedicalHistoryEntry>(entity =>
        {
            entity.ToTable("MedicalHistoryEntries");
            entity.HasKey(h => h.Id);
            entity.Property(h => h.EntryType).HasMaxLength(80);
            entity.Property(h => h.Notes).HasColumnType("nvarchar(max)");
            entity.HasIndex(h => h.PatientId);
            entity.HasIndex(h => h.PhysicianId);
            entity.HasIndex(h => h.AppointmentId);
            entity.HasIndex(h => h.TreatmentId);
            entity.HasOne<MedicalTreatment>()
                .WithMany()
                .HasForeignKey(h => h.TreatmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.ToTable("Prescriptions");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.MedicationDetails).HasColumnType("nvarchar(max)");
            entity.HasIndex(p => p.PatientId);
            entity.HasIndex(p => p.PhysicianId);
            entity.HasIndex(p => p.AppointmentId);
            entity.HasIndex(p => p.TreatmentId);
            entity.HasOne<MedicalTreatment>()
                .WithMany()
                .HasForeignKey(p => p.TreatmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
