using Jg.wpf.controls.Customer.Autocompletes;
using System.Collections.Generic;
using System.Data;
using System.Windows.Controls;
using Calculator.Model.Models;
using Calculator.Service.Services.Database;
using Jg.wpf.core.Service;
using System;
using System.Windows.Input;
using Calculator.ViewModel.ViewModels.Applications;

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

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (sender is TextBox textBox)
                {
                    if (string.IsNullOrEmpty(textBox.Text))
                    {
                        if (DataContext is MainWindowViewModel vm)
                        {
                            vm.InitPatients();
                        }
                    }
                }
            }
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
            if (string.IsNullOrEmpty(searchPatientName))
            {
                return null;
            }

            var dataTable = _dbService.GetPatientNames(searchPatientName);

            var patientNames = new List<string>();
            foreach (DataRow row in dataTable.Rows)
            {
                var name = row["name"].ToString();
                patientNames.Add(name);
            }

            return patientNames;
        }
    }
}
