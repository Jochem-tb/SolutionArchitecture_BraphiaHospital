using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models;

public class HealthQuestionaire
{
    public required Guid Id { get; set; }
    public required Guid AppointmentId { get; set; }
    public required string HealthDataJSON { get; set; }
    public required Appointment Appointment { get; set; }
}