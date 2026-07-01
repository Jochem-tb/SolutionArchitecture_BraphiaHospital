using PatientCare.Domain.Read;
using Microsoft.EntityFrameworkCore;

namespace PatientCare.Infrastructure.ReadDb;

public class PatientCareReadDbContext : DbContext
{
    public PatientCareReadDbContext(DbContextOptions<PatientCareReadDbContext> options)
        : base(options)
    {
    }

    public DbSet<PatientDossierReadModel> PatientDossiers => Set<PatientDossierReadModel>();
    public DbSet<MedicalHistoryReadItem> MedicalHistoryItems => Set<MedicalHistoryReadItem>();
    public DbSet<PrescriptionReadItem> ActivePrescriptionItems => Set<PrescriptionReadItem>();
    public DbSet<AppointmentReadItem> UpcomingAppointmentItems => Set<AppointmentReadItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PatientDossierReadModel>(entity =>
        {
            entity.ToTable("PatientDossiers");
            entity.HasKey(e => e.PatientId);
            entity.Property(e => e.PatientName).HasMaxLength(200);
            entity.HasMany(e => e.MedicalHistory)
                .WithOne()
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.ActivePrescriptions)
                .WithOne()
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(e => e.UpcomingAppointments)
                .WithOne()
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MedicalHistoryReadItem>(entity =>
        {
            entity.ToTable("MedicalHistoryReadItems");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Notes).HasColumnType("nvarchar(max)");
            entity.HasIndex(e => e.PatientId);
            entity.HasIndex(e => e.Timestamp);
        });

        modelBuilder.Entity<PrescriptionReadItem>(entity =>
        {
            entity.ToTable("PrescriptionReadItems");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MedicationDetails).HasColumnType("nvarchar(max)");
            entity.HasIndex(e => e.PatientId);
        });

        modelBuilder.Entity<AppointmentReadItem>(entity =>
        {
            entity.ToTable("AppointmentReadItems");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.PatientId);
            entity.HasIndex(e => e.AppointmentId);
        });
    }
}
