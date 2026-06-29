using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models;

public class PatientSmall
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public ICollection<Appointment> Appointments { get; set; } = [];
}