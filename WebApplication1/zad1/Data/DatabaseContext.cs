using Microsoft.EntityFrameworkCore;
using zad1.Models;

namespace zad1.Data;

public class DatabaseContext : DbContext
{
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }

    protected DatabaseContext()
    {
    }

    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Doctor>(d =>
        {
            d.ToTable("Doctor");

            d.HasKey(e => e.IdDoctor);
            d.Property(e => e.FirstName).HasMaxLength(100);
            d.Property(e => e.LastName).HasMaxLength(100);
            d.Property(e => e.Email).HasMaxLength(100);
        });

        modelBuilder.Entity<Patient>(p =>
        {
            p.ToTable("Patient");

            p.HasKey(e => e.IdPatient);
            p.Property(e => e.FirstName).HasMaxLength(100);
            p.Property(e => e.LastName).HasMaxLength(100);
            p.Property(e => e.BirthDate).HasColumnType("date");
        });

        modelBuilder.Entity<Prescription>(pr =>
        {
            pr.ToTable("Prescription");

            pr.HasKey(e => e.IdPrescription);
            pr.Property(e => e.Date).HasColumnType("date");
            pr.Property(e => e.DueDate).HasColumnType("date");
            pr.Property(e => e.IdPatient);
            pr.Property(e => e.IdDoctor);
        });
    }
}