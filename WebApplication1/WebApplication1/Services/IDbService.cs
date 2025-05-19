using zad1.DTOs;

namespace zad1.Services;

public interface IDbService
{
    Task AddPrescription(NewPrescriptionDto prescription);
    
    Task<GetPatientDto> GetPatient(int id);
}