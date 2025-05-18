using Microsoft.EntityFrameworkCore;
using zad1.Data;
using zad1.DTOs;
using zad1.Exceptions;
using zad1.Models;

namespace zad1.Services;

public class DbService : IDbService
{
    private readonly DatabaseContext _context;

    public DbService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task AddPrescription(NewPrescriptionDTO prescription)
    {
        var doctor = await _context.Doctors
            .Where(p => p.IdDoctor == prescription.Doctor.IdDoctor).ToListAsync();
        if (!doctor.Any())
        {
            throw new NotFoundException("Doctor not found");
        }

        if (prescription.Medicaments.Count > 10)
        {
            throw new BadRequestException("Medicaments count exceeded");
        }

        if (prescription.DueDate < prescription.Date)
        {
            throw new BadRequestException("Due date cannot be earlier than date");
        }
        
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var patient = await _context.Patients
                .Where(p => p.IdPatient == prescription.Patient.IdPatient).ToListAsync();
            if (!patient.Any())
            {
                _context.Patients.Add(new Patient()
                {
                    IdPatient = prescription.Patient.IdPatient,
                    FirstName = prescription.Patient.FirstName,
                    LastName = prescription.Patient.LastName,
                    BirthDate = prescription.Patient.BirthDate
                });
                await _context.SaveChangesAsync();
            }
            
            var p = new Prescription()
            {
                Date = prescription.Date,
                DueDate = prescription.DueDate,
                IdPatient = prescription.Patient.IdPatient,
                IdDoctor = prescription.Doctor.IdDoctor
            };
            
            _context.Prescriptions.Add(p);
            await _context.SaveChangesAsync();

            foreach (var m in prescription.Medicaments)
            {
                var exists = _context.Medicaments.Any(me => me.IdMedicament == m.IdMedicament);
                if (!exists)
                {
                    throw new NotFoundException("Medicament not found");
                }

                _context.PrescriptionMedicaments.Add(new PrescriptionMedicament()
                {
                    IdPrescription = p.IdPrescription,
                    IdMedicament = m.IdMedicament,
                    Dose = m.Dose ?? null,
                    Details = m.Details
                });
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }

    }
}