using System;
using System.Data;
using Calculator.Model.Models;

namespace Calculator.Service.Services.Database
{
    public interface ISQLiteDataService
    {
        DataTable GetPatients();
        void AddPatient(string id, string name, DateTime birthday, double weight);
        void UpdatePatientInfo(string patientId, string patientName, DateTime patientBirthday, double patientWeight);
        void DeletePatient(string id);

        DataTable GetPatientVariables(string id);
        void DeletePatientVariables(string patientId);
        void InsertPatientVariable(string patientId, Variable variable);
    }
}
