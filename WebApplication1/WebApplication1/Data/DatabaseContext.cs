using Microsoft.EntityFrameworkCore;
using zad1.Models;

namespace zad1.Data;

public class DatabaseContext : DbContext
{
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Medicament> Medicaments { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<PrescriptionMedicament> PrescriptionMedicaments { get; set; }

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

        modelBuilder.Entity<Medicament>(m =>
        {
            m.ToTable("Medicament");

            m.HasKey(e => e.IdMedicament);
            m.Property(e => e.Name).HasMaxLength(100);
            m.Property(e => e.Description).HasMaxLength(100);
            m.Property(e => e.Type).HasMaxLength(100);
        });

        modelBuilder.Entity<PrescriptionMedicament>(pm =>
        {
            pm.ToTable("Prescription_Medicament");
            pm.HasKey(e => new { e.IdMedicament, e.IdPrescription });
            pm.Property(e => e.Dose);
            pm.Property(e => e.Details).HasMaxLength(100);
        });

        modelBuilder.Entity<Doctor>().HasData(new List<Doctor>()
        {
            new Doctor() { IdDoctor = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" },
            new Doctor() { IdDoctor = 2, FirstName = "Jane", LastName = "Doe", Email = "jane.doe@example.com" }
        });
        
        modelBuilder.Entity<Patient>().HasData(new List<Patient>()
        {
            new Patient() { IdPatient = 1, FirstName = "Jonathan", LastName = "Boe", BirthDate = new DateTime(1999, 5, 2)},
            new Patient() { IdPatient = 2, FirstName = "Ann", LastName = "Boe", BirthDate = new DateTime(1980, 1, 19)}
        });
        
        modelBuilder.Entity<Medicament>().HasData(new List<Medicament>()
        {
            new Medicament() { IdMedicament = 1, Name = "Aspirin", Description = "Only 1 per day", Type = "Treat pain"},
            new Medicament() { IdMedicament = 2, Name = "Ibuprom", Description = "Max 3 per day", Type = "Treat headache"}
        });
    }
}