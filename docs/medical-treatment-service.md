# Medical Treatment Service

De Medical Treatment Service slaat alleen data op die deze microservice zelf nodig heeft. Andere gegevens, zoals patiëntgegevens, afspraken, verwijzingen, huisartsen, artsprofielen, facturen en labdetails, blijven eigendom van andere microservices.

## Eigen verantwoordelijkheid

Deze service is verantwoordelijk voor:

- medische behandeling/consult registreren;
- medische historie en artsnotities opslaan;
- ingevulde gezondheidsvragenlijsten als dossier-entry opslaan;
- medicatie voorschrijven;
- verzoeken voor verdere analyse als event publiceren;
- analyse-/labresultaten als dossier-entry opslaan wanneer ze via events terugkomen;
- declarabele medische acties als event publiceren naar Billing.

## Bewust niet lokaal opgeslagen

Deze microservice heeft geen eigen tabellen voor:

- `Patient`;
- `Physician`;
- `GeneralPractitioner`;
- `Referral`;
- `Appointment`;
- `HealthQuestionnaire` als aparte tabel;
- `Invoice`;
- `ProcedureRequest`;
- `ProcedureResult`.

Van externe concepten worden alleen IDs gebruikt op de eigen medische records, bijvoorbeeld `PatientId`, `AppointmentId` en `PhysicianId`.

## Eigen datamodel

| Eigen entity | Tabel | Data |
| --- | --- | --- |
| `MedicalTreatment` | `MedicalTreatments` | `Id`, `PatientId`, `AppointmentId`, `PhysicianId`, `Type`, `Results`, `Status`, `CreatedAt`, `ExaminedAt` |
| `MedicalHistoryEntry` | `MedicalHistoryEntries` | `Id`, `PatientId`, `TreatmentId`, `AppointmentId`, `PhysicianId`, `EntryType`, `Notes`, `Timestamp` |
| `Prescription` | `Prescriptions` | `Id`, `PatientId`, `TreatmentId`, `AppointmentId`, `PhysicianId`, `MedicationDetails`, `PharmacyNotified`, `PrescribedAt`, `ValidUntil` |

## Events die de service consumeert

| Event | Gebruik |
| --- | --- |
| `AppointmentRescheduled` | Werkt alleen de referentievelden van een bestaande behandeling bij. Er wordt geen afspraak opgeslagen. |
| `PatientCheckedIn` | Maakt of werkt een `MedicalTreatment` aan voor de afspraak. |
| `SurveyCompleted` | Slaat de vragenlijst op als `MedicalHistoryEntry` met type `HealthQuestionnaire`. |
| `ProcedureDone` | Slaat het resultaat op als `MedicalHistoryEntry` met type `ProcedureResult` en werkt de behandeling bij. |

## Events die de service publiceert

| Event | Doel |
| --- | --- |
| `PatientExamined` | Geeft door dat het consult/onderzoek is uitgevoerd. |
| `MedicalHistoryAdded` | Geeft door dat het dossier is aangevuld. |
| `MedicationPrescribed` | Geeft door dat de apotheek geïnformeerd moet worden over medicatie. |
| `ProcedureRequested` | Stuurt verdere analyse, zoals bloedtest of röntgen, door naar de Medical Procedure/Lab service. |
| `LabResultAssociated` | Geeft door dat een resultaat aan patiënt/afspraak/behandeling is gekoppeld. |
| `MedicalActivityRegistered` | Geeft declarabele medische acties door aan Billing. |

## Belangrijkste endpoints

- `GET /medical-treatments/{treatmentId}`
- `GET /medical-treatments/patients/{patientId}/history`
- `POST /medical-treatments/exams`
- `POST /medical-treatments/{treatmentId}/notes`
- `POST /medical-treatments/{treatmentId}/prescriptions`
- `POST /medical-treatments/{treatmentId}/procedure-requests`

## Voorbeeld exam request

```json
{
  "appointmentId": "11111111-1111-1111-1111-111111111111",
  "patientId": "22222222-2222-2222-2222-222222222222",
  "physicianId": "33333333-3333-3333-3333-333333333333",
  "treatmentType": "Consult",
  "results": "Patiënt onderzocht, verdere bloedtest nodig.",
  "notes": "Patiënt meldt pijn op de borst bij inspanning."
}
```

## Voorbeeld prescription request

```json
{
  "medicationDetails": "Paracetamol 500mg, maximaal 3 keer per dag",
  "validUntil": "2026-07-29T00:00:00Z"
}
```
