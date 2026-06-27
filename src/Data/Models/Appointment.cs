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
    public Guid? HealthQuestionaireId { get; set; }
    public required DateTime DateTime { get; set; }
    public required AppointmentArrivalStatus ArrivalStatus { get; set; }
    public required AppointmentScheduleStatus ScheduleStatus { get; set; }
    public required PatientSmall Patient {  get; set; }
    public HealthQuestionaire? HealthQuestionaire { get; set; }
}