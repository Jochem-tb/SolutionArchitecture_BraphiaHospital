using Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models;

public class Appointment
{
    public required Guid Id { get; set; }
    public required Guid PatientId { get; set; }
    public required Guid PhysicianId { get; set; }
    public required Guid ReferalId { get; set; }
    public required DateTime DateTime { get; set; }
    public required AppointmentArrivalStatus ArrivalStatus { get; set; }
    public required AppointmentScheduleStatus ScheduleStatus { get; set; }
    public PatientSmall Patient { get; set; } = null!;
    public HealthQuestionaire? HealthQuestionaire { get; set; }

    public void AddOrUpdateQuestionnaire(string json)
    {
        if (HealthQuestionaire == null)
        {
            HealthQuestionaire = new HealthQuestionaire
            {
                HealthDataJSON = json
            };
        }
        else
        {
            HealthQuestionaire.HealthDataJSON = json;
        }
    }
}