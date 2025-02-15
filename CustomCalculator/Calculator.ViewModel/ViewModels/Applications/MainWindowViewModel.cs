using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using Calculator.Model.Events;
using Calculator.Model.Models;
using Calculator.Service.Services.Database;
using Calculator.Service.Services.Parser;
using Calculator.Service.Services.Patients;
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
        private bool _isDialogOpen;
        private readonly ISQLiteDataService _dbService;

        public object DialogViewModel
        {
            get => _dialogViewModel;
            set
            {
                _dialogViewModel = value;
                IsDialogOpen = true;
                RaisePropertyChanged(nameof(DialogViewModel));
            }
        }

        public bool IsDialogOpen
        {
            get => _isDialogOpen;
            set
            {
                if (value == _isDialogOpen) return;
                _isDialogOpen = value;
                RaisePropertyChanged(nameof(IsDialogOpen));
            }
        }

        public ObservableCollection<Patient> Patients { get; }
        public Patient SelectPatient { get; set; }

        public JCommand AddPatientCommand { get; }
        public JCommand EditPatientCommand { get; }


        public AddPatientViewModel AddPatientViewModel { get; }
        public EditPatientViewModel EditPatientViewModel { get; }
        public MessageViewModel MessageViewModel { get; }
        

        public FormulaConfigViewModel FormulaConfigViewModel { get; }
        public MainWindowViewModel()
        {
            _parser = ServiceManager.GetService<IParser>();
            _dbService = ServiceManager.GetService<ISQLiteDataService>();
            
            AddPatientCommand = new JCommand("AddPatientCommand", OnAddPatient);
            EditPatientCommand = new JCommand("EditPatientCommand", OnEditPatient);

            AddPatientViewModel = new AddPatientViewModel();
            AddPatientViewModel.OnPatientAdded += OnPatientAdded;

            EditPatientViewModel = new EditPatientViewModel();
            EditPatientViewModel.OnPatientEdited += OnPatientEdited;

            MessageViewModel = new MessageViewModel();
            MessageViewModel.OnMessageClosed += OnMessageClosed;

            Patients = new ObservableCollection<Patient>();

            //var patient = new Patient();
            //patient.Test();

            //FormulaConfigViewModel = new FormulaConfigViewModel(patient.Formulas[0].Variables.ToList());

            //var expression = patient.Build();

            //Console.WriteLine(_parser.Parse(expression));

            var patients = _dbService.GetPatients();
            foreach (DataRow row in patients.Rows)
            {
                var name = row["name"].ToString();
                var birthday = row["birthday"].ToString();
                var weight = row["weight"].ToString();

                Patients.Add(new Patient(name, DateTime.Parse(birthday), double.Parse(weight)));
            }
        }

        private void OnMessageClosed(object sender, EventArgs e)
        {
            IsDialogOpen = false;
        }


        private void OnEditPatient(object obj)
        {
            if (SelectPatient == null)
            {
                MessageViewModel.SetMessage("请先选择一个病人信息");
                DialogViewModel = MessageViewModel;
                return;
            }

            EditPatientViewModel.SetPatient((Patient)SelectPatient.Clone());
            DialogViewModel = EditPatientViewModel;
        }

        private void OnAddPatient(object obj)
        {
            AddPatientViewModel.SetPatient();
            DialogViewModel = AddPatientViewModel;
        }

        private void OnPatientAdded(object sender, PatientAddOrEditEventArgs args)
        {
            if (!args.IsCancel)
            {
                Patients.Add(args.Patient);
            }

            _dbService.AddPatient(args.Patient.Name, args.Patient.Birthday, args.Patient.Weight);

            IsDialogOpen = false;
        }
        private void OnPatientEdited(object sender, PatientAddOrEditEventArgs args)
        {
            if (!args.IsCancel)
            {
                var patient = Patients.FirstOrDefault(p => p.Id == args.Patient.Id);
                if (patient != null)
                {
                    patient.Name = args.Patient.Name;
                    patient.Birthday = args.Patient.Birthday;
                    patient.Weight = args.Patient.Weight;
                }
            }

            IsDialogOpen = false;
        }

    }
}
