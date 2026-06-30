using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models;

public class HealthQuestionaire
{
    public Guid Id { get; set; }
    public Guid AppointmentId { get; set; }
    public required string HealthDataJSON { get; set; }
    public Appointment Appointment { get; set; } = null!;
}