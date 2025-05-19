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

    public async Task AddPrescription(NewPrescriptionDto prescription)
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
            await transaction.CommitAsync();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<GetPatientDto> GetPatient(int id)
    {
        var patient = await _context.Patients
            .Where(p => p.IdPatient == id)
            .Include(p => p.Prescriptions)
                .ThenInclude(pr => pr.PrescriptionMedicaments)
                    .ThenInclude(pm => pm.Medicament)
            .Include(p => p.Prescriptions)
                .ThenInclude(pr => pr.Doctor)
            .FirstOrDefaultAsync();

        if (patient == null)
        {
            throw new NotFoundException("Patient not found");
        }

        var result = new GetPatientDto()
        {
            IdPatient = patient.IdPatient,
            FirstName = patient.FirstName,
            LastName = patient.LastName,
            BirthDate = patient.BirthDate,
            Prescriptions = patient.Prescriptions
                .OrderBy(pr => pr.DueDate)
                .Select(pr => new GetPrescriptionDto()
                {
                    IdPrescription = pr.IdPrescription,
                    Date = pr.Date,
                    DueDate = pr.DueDate,
                    Medicaments = pr.PrescriptionMedicaments
                        .Select(pm => new GetMedicamentDto()
                        {
                            IdMedicament = pm.IdMedicament,
                            Name = pm.Medicament.Name,
                            Dose = pm.Dose,
                            Description = pm.Details
                        }).ToList(),
                    Doctor = new GetDoctorDto()
                    {
                        IdDoctor = pr.IdDoctor,
                        FirstName = pr.Doctor.FirstName
                    }
                }).ToList()
        };
        
        return result;
    }
}