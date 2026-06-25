using System.ComponentModel.DataAnnotations;

namespace Data.OldModels;

public sealed class BsnPseudoniem
{
    public required int Id { get; set; }

    [Required]
    public required string Bsn { get; set; }

    [Required]
    public required string Pseudoniem { get; set; }
}
