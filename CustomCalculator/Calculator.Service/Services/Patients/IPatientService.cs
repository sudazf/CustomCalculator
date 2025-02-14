using Calculator.Model.Models;

namespace Calculator.Service.Services.Patients
{
    public interface IPatientService
    {
        Patient AddPatient();
        void UpdatePatient(Patient patient);
    }
}
