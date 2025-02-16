using System;
using System.Data;

namespace Calculator.Service.Services.Database
{
    public interface ISQLiteDataService
    {
        DataTable GetPatients();
        void AddPatient(string id, string name, DateTime birthday, double weight);
        void UpdatePatientInfo(string patientId, string patientName, DateTime patientBirthday, double patientWeight);
        void AddCalculation(string calcId, string patientId, string p1, string p2, string p3, string p4, string formula);
    }
}
