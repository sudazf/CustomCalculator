using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using Calculator.Model.Models;
using Calculator.Service.Services.Database;
using Jg.wpf.core.Service;

namespace Calculator.ViewModel.Helpers
{
    public class PatientDataHelper
    {
        private readonly ISQLiteDataService _dbService;

        public PatientDataHelper()
        {
            _dbService = ServiceManager.GetService<ISQLiteDataService>();

        }

        public IEnumerable<Patient> GetAllPatients()
        {
            try
            {
                var dataTable = _dbService.GetPatients();
                if (dataTable == null)
                {
                    return null;
                }

                var patients = new List<Patient>();
                foreach (DataRow row in dataTable.Rows)
                {
                    var id = row["id"].ToString();
                    var name = row["name"].ToString();
                    var birthday = row["birthday"].ToString();
                    var weight = row["weight"].ToString();

                    patients.Add(new Patient(id, name, DateTime.Parse(birthday), double.Parse(weight)));
                }

                return patients;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public IEnumerable<Variable> GetPatientVariables(string patientId)
        {
            try
            {
                var dataTable = _dbService.GetPatientVariables(patientId);
                if (dataTable == null)
                {
                    return null;
                }

                var patientVariables = new List<Variable>();
                foreach (DataRow row in dataTable.Rows)
                {
                    var id = row["id"].ToString();
                    var name = row["variable_name"].ToString();
                    var value = row["variable_value"].ToString();
                    var min = row["variable_min"].ToString();
                    var max = row["variable_max"].ToString();
                    var unit = row["variable_unit"].ToString();
                    var metaExpression = row["variable_expression"].ToString();

                    patientVariables.Add(new Variable(id, name, value, unit, min, max, new Formula(metaExpression)));
                }

                foreach (var variable in patientVariables)
                {
                    var expressionItems = variable.Formula.ExpressionItems;
                    var names = variable.Formula.MetaExpression.Split(',');
                    foreach (var name in names)
                    {
                        var existVariable = patientVariables.FirstOrDefault(v => v.Name == name);
                        expressionItems.Add(new ExpressionItem(name, existVariable == null ? name : existVariable.Value));
                    }
                }

                return patientVariables;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void SavePatientVariables(string patientId, ObservableCollection<Variable> patientVariables)
        {
            try
            {
                _dbService.DeletePatientVariables(patientId);
                foreach (var variable in patientVariables)
                {
                    _dbService.InsertPatientVariable(patientId, variable);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
