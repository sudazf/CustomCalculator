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

        DataTable GetPatientDays(string id);
        void DeletePatientDailyVariables(string patientId, string day);
        void InsertPatientDailyVariable(string patientId, string day,Variable variable);
        void InsertVariableTemplate(VariableTemplate template);
        DataTable GetVariableTemplates(string templateName);
        DataTable GetTemplateNames();
        void DeletePatientDays(string id);
    }
}
