using PatientCare.Domain.Write;
using Microsoft.EntityFrameworkCore;

namespace PatientCare.Infrastructure.WriteDb;

public class PatientCareWriteDbContext : DbContext
{
    public PatientCareWriteDbContext(DbContextOptions<PatientCareWriteDbContext> options)
        : base(options)
    {
    }

    public DbSet<MedicalHistoryEntry> MedicalHistoryEntries => Set<MedicalHistoryEntry>();
    public DbSet<Prescription> Prescriptions => Set<Prescription>();
    public DbSet<PatientCache> Patients => Set<PatientCache>();
    public DbSet<AppointmentCache> Appointments => Set<AppointmentCache>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MedicalHistoryEntry>(entity =>
        {
            entity.ToTable("MedicalHistoryEntries");
            entity.HasKey(e => e.EntryId);
            entity.Property(e => e.Notes).HasColumnType("nvarchar(max)");
            entity.HasIndex(e => e.PatientId);
            entity.HasIndex(e => e.PhysicianId);
            entity.HasIndex(e => e.Timestamp);
        });

        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.ToTable("Prescriptions");
            entity.HasKey(e => e.PrescriptionId);
            entity.Property(e => e.MedicationDetails).HasColumnType("nvarchar(max)");
            entity.HasIndex(e => e.PatientId);
        });

        modelBuilder.Entity<PatientCache>(entity =>
        {
            entity.ToTable("PatientCache");
            entity.HasKey(e => e.PatientId);
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<AppointmentCache>(entity =>
        {
            entity.ToTable("AppointmentCache");
            entity.HasKey(e => e.AppointmentId);
            entity.HasIndex(e => e.PatientId);
            entity.HasIndex(e => e.PhysicianId);
        });
    }
}
