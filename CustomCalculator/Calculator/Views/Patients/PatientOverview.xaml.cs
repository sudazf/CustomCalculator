using Jg.wpf.controls.Customer.Autocompletes;
using System.Collections.Generic;
using System.Data;
using System.Windows.Controls;
using Calculator.Model.Models;
using Calculator.Service.Services.Database;
using Jg.wpf.core.Service;
using System;
using System.Windows.Input;

namespace Calculator.Views.Patients
{
    /// <summary>
    /// PatientOverview.xaml 的交互逻辑
    /// </summary>
    public partial class PatientOverview : UserControl
    {

        public PatientOverview()
        {
            InitializeComponent();

            PatientSuggestionBox.TextBoxSuggestionsSource = new PatientSuggestionsSource();
        }

    }

    public class PatientSuggestionsSource : TextBoxSuggestionsSource
    {
        private readonly ISQLiteDataService _dbService;

        public PatientSuggestionsSource()
        {
            _dbService = ServiceManager.GetService<ISQLiteDataService>();
        }

        public override IEnumerable<string> Search(string searchPatientName)
        {
            searchPatientName = searchPatientName ?? string.Empty;
            var dataTable = _dbService.GetPatients(searchPatientName);

            var patientNames = new List<string>();
            var patients = new List<Patient>();
            foreach (DataRow row in dataTable.Rows)
            {
                var id = row["id"].ToString();
                var bedNumber = row["bed_number"].ToString();
                var name = row["name"].ToString();
                var birthday = row["birthday"].ToString();
                var weight = row["weight"].ToString();
                var diagnosis = row["diagnosis"].ToString();

                patientNames.Add(name);
                patients.Add(new Patient(id, bedNumber, name, DateTime.Parse(birthday), double.Parse(weight), diagnosis));
            }

            return patientNames;
        }
    }
}
