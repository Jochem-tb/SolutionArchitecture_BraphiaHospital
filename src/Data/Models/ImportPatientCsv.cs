namespace Data.Models;

public class ImportPatientCsv
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string PhoneNumber { get; init; }
    public required string CompanyName { get; init; }
}