using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using Calculator.Model.Events;
using Calculator.Model.Models;
using Calculator.Service.Services.App;
using Calculator.Service.Services.Database;
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
        private readonly IWindowService _windowService;
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
        public JCommand EditPatientVariablesCommand { get; }
        public JCommand CalculateCommand { get; }

        public AddPatientViewModel AddPatientViewModel { get; }
        public EditPatientViewModel EditPatientViewModel { get; }
        public PatientVariablesSettingViewModel PatientVariablesSettingViewModel { get; }
        public CalculateViewModel CalculateViewModel { get; }
        public MessageViewModel MessageViewModel { get; }
        
        public MainWindowViewModel()
        {
            _parser = ServiceManager.GetService<IParser>();
            _dbService = ServiceManager.GetService<ISQLiteDataService>();
            _windowService = ServiceManager.GetService<IWindowService>();

            AddPatientCommand = new JCommand("AddPatientCommand", OnAddPatient);
            EditPatientCommand = new JCommand("EditPatientCommand", OnEditPatient);
            EditPatientVariablesCommand = new JCommand("EditPatientVariablesCommand", OnEditPatientVariables);
            
            CalculateCommand = new JCommand("CalculateCommand", OnCalc);
            
            AddPatientViewModel = new AddPatientViewModel();
            AddPatientViewModel.OnPatientAdded += OnPatientAdded;

            EditPatientViewModel = new EditPatientViewModel();
            EditPatientViewModel.OnPatientEdited += OnPatientEdited;

            PatientVariablesSettingViewModel = new PatientVariablesSettingViewModel();
            PatientVariablesSettingViewModel.OnSettingCompleted += OnPatientVariablesSettingCompleted;

            CalculateViewModel = new CalculateViewModel();
            CalculateViewModel.OnCalculate += OnCalculated;

            MessageViewModel = new MessageViewModel();
            MessageViewModel.OnMessageClosed += OnMessageClosed;

            Patients = new ObservableCollection<Patient>();

            var patients = _dbService.GetPatients();
            foreach (DataRow row in patients.Rows)
            {
                var name = row["name"].ToString();
                var birthday = row["birthday"].ToString();
                var weight = row["weight"].ToString();

                Patients.Add(new Patient(name, DateTime.Parse(birthday), double.Parse(weight)));
            }
        }

        private void OnPatientVariablesSettingCompleted(object sender, ConfirmEventArgs args)
        {
            _windowService.Close();
        }

        private void OnCalculated(object sender, PatientCalculateEventArgs e)
        {
            if (!e.IsCancel)
            {
                var calcId = Guid.NewGuid().ToString();

            }
            IsDialogOpen = false;
        }

        /// <summary>
        /// 计算病人数据
        /// </summary>
        /// <param name="obj"></param>
        private void OnCalc(object obj)
        {
            if (SelectPatient == null)
            {
                MessageViewModel.SetMessage("请先选择一个病人信息");
                DialogViewModel = MessageViewModel;
                return;
            }

            CalculateViewModel.SetPatient((Patient)SelectPatient.Clone());
            DialogViewModel = CalculateViewModel;
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

        private void OnEditPatientVariables(object obj)
        {
            if (SelectPatient == null)
            {
                MessageViewModel.SetMessage("请先选择一个病人信息");
                DialogViewModel = MessageViewModel;
                return;
            }

            PatientVariablesSettingViewModel.SetPatient(SelectPatient);
            _windowService.ShowDialog("EditPatientVariables", PatientVariablesSettingViewModel);
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

                _dbService.AddPatient(args.Patient.Id, args.Patient.Name, args.Patient.Birthday, args.Patient.Weight);
            }

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

                    _dbService.UpdatePatientInfo(patient.Id, patient.Name, patient.Birthday, patient.Weight);
                }
            }

            IsDialogOpen = false;
        }

    }
}
