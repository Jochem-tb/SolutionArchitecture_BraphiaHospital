using Data.Models;
using Data.OldModels;
using Microsoft.EntityFrameworkCore;

namespace Data.DbContexts;

public class AppointmentDbContext : DbContext
{
    public AppointmentDbContext()
    {

    }

    public AppointmentDbContext(DbContextOptions<AppointmentDbContext> options) : base(options)
    {

    }

    public DbSet<PatientSmall> Patients { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<HealthQuestionaire> HealthQuestionaires { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.HealthQuestionaire)
            .WithOne(h => h.Appointment)
            .HasForeignKey<Appointment>(a => a.HealthQuestionaireId);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PatientId);
    }
}