namespace zad1.DTOs;

public class NewPrescriptionDTO
{
    public NewPatientDTO Patient { get; set; }
    public List<MedicamentDTO> Medicaments { get; set; }
    public DateTime Date { get; set; }
    public DateTime DueDate { get; set; }
}

public class NewPatientDTO
{
    public int IdPatient { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
}

public class MedicamentDTO
{
    public int IdMedicament { get; set; }
    public int Dose { get; set; }
    public string Description { get; set; }
}