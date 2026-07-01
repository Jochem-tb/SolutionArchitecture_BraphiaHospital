namespace Data.Models;

public class Patient
{
    public required Guid PatientId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string PhoneNumber { get; set; }
    public required string InsuranceNumber { get; set; }
}