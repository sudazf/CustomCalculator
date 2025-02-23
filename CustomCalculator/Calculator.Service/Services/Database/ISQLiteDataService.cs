using System;
using System.Data;
using Calculator.Model.Models;

namespace Calculator.Service.Services.Database
{
    public interface ISQLiteDataService
    {
        DataTable GetPatientsCount();
        DataTable GetPatients(int offset, int limit);
        DataTable GetPatientsCount(string patientName);
        DataTable GetPatients(string patientName, int offset, int limit);
        DataTable GetPatientNames(string searchPatientName);

        void AddPatient(string id, string bedNumber, string name, DateTime birthday, double weight);
        void UpdatePatientInfo(string patientId, string bedNumber, string patientName, 
            DateTime patientBirthday, double patientWeight, string diagnosis);
        void DeletePatient(string id);

        DataTable GetPatientDays(string id);
        void DeletePatientDailyVariables(string patientId, string day);
        void InsertPatientDailyVariable(string patientId, string day,Variable variable);
        void InsertVariableTemplate(VariableTemplate template);
        DataTable GetVariableTemplates(string templateName);
        DataTable GetTemplateNames();
        void DeletePatientDays(string id);
    }
}
