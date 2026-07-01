using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.DbContexts;

public class PatientDbContext : DbContext
{

    public PatientDbContext(DbContextOptions<PatientDbContext> options)
        : base(options)
    {}
    public DbSet<Patient> Patients { get; set; } 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(p => p.PatientId);
            entity.HasIndex(p => p.InsuranceNumber).IsUnique();
        });
    }
}