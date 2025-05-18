using zad1.DTOs;

namespace zad1.Services;

public interface IDbService
{
    Task AddPrescription(NewPrescriptionDTO prescription);
}