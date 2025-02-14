using System.Collections.ObjectModel;
using Calculator.Model.Models;
using Calculator.Service.Services.Parser;
using Calculator.ViewModel.ViewModels.Formulas;
using Calculator.ViewModel.ViewModels.Patients;
using Jg.wpf.core.Command;
using Jg.wpf.core.Notify;
using Jg.wpf.core.Service;

namespace Calculator.ViewModel.ViewModels.Applications
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IParser _parser;
        private object _dialogViewModel;
        public ObservableCollection<Patient> Patients { get; }

        public JCommand AddPatientCommand { get; }

        public object DialogViewModel
        {
            get => _dialogViewModel;
            set
            {
                if (Equals(value, _dialogViewModel)) return;
                _dialogViewModel = value;
                RaisePropertyChanged(nameof(DialogViewModel));
            }
        }

        public AddPatientViewModel AddPatientViewModel { get; }
        public FormulaConfigViewModel FormulaConfigViewModel { get; }
        public MainWindowViewModel()
        {
            _parser = ServiceManager.GetService<IParser>();

            AddPatientCommand = new JCommand("AddPatientCommand", OnAddPatient);

            AddPatientViewModel = new AddPatientViewModel();
            AddPatientViewModel.OnAddedPatient += OnAddNewPatient;

            Patients = new ObservableCollection<Patient>();

            //var patient = new Patient();
            //patient.Test();

            //FormulaConfigViewModel = new FormulaConfigViewModel(patient.Formulas[0].Variables.ToList());

            //var expression = patient.Build();

            //Console.WriteLine(_parser.Parse(expression));
        }

        private void OnAddPatient(object obj)
        {
            DialogViewModel = AddPatientViewModel;
            AddPatientViewModel.IsAddingPatient = true;
        }

        private void OnAddNewPatient(object sender, Patient newPatient)
        {
            Patients.Add(newPatient);

            DialogViewModel = null;
        }
    }
}
