using System;
using System.Data;

namespace Calculator.Service.Services.Database
{
    public interface ISQLiteDataService
    {
        DataTable GetPatients();
        void AddPatient(string id, string name, DateTime birthday, double weight);
        void UpdatePatientInfo(string patientId, string patientName, DateTime patientBirthday, double patientWeight);
    }
}
