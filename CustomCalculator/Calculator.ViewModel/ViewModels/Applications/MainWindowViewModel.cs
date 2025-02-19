using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Calculator.Model.Events;
using Calculator.Model.Models;
using Calculator.Service.Services.Database;
using Calculator.ViewModel.Helpers;
using Calculator.ViewModel.ViewModels.Patients;
using Jg.wpf.core.Command;
using Jg.wpf.core.Notify;
using Jg.wpf.core.Profilers;
using Jg.wpf.core.Service;

namespace Calculator.ViewModel.ViewModels.Applications
{
    public class MainWindowViewModel : ViewModelBase
    {
        private object _dialogViewModel;
        private bool _isDialogOpen;
        private readonly ISQLiteDataService _dbService;
        private readonly IDispatcher _dispatcher;
        private Patient _selectPatient;
        private readonly PatientDataHelper _dataHelper;

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
        public Patient SelectPatient
        {
            get => _selectPatient;
            set
            {
                if (Equals(value, _selectPatient)) return;
                _selectPatient = value;
                InitSelectPatientVariables();
                RaisePropertyChanged(nameof(SelectPatient));
            }
        }

        public JCommand AddPatientCommand { get; }
        public JCommand EditPatientCommand { get; }
        public JCommand EditVariableExpressionCommand { get;  }
        public JCommand SaveVariablesCommand { get; }
        public JCommand CancelSaveVariablesCommand { get; }

        public AddPatientViewModel AddPatientViewModel { get; }
        public EditPatientViewModel EditPatientViewModel { get; }
        public CalculateViewModel CalculateViewModel { get; }
        public VariableExpressionViewModel VariableExpressionViewModel { get; }
        public MessageViewModel MessageViewModel { get; }
        
        public MainWindowViewModel()
        {
            _dbService = ServiceManager.GetService<ISQLiteDataService>();
            _dispatcher = ServiceManager.GetService<IDispatcher>();
            _dataHelper = new PatientDataHelper();

            AddPatientCommand = new JCommand("AddPatientCommand", OnAddPatient);
            EditPatientCommand = new JCommand("EditPatientCommand", OnEditPatient);
            EditVariableExpressionCommand = new JCommand("EditVariableExpressionCommand", OnEditVariableExpression);
            SaveVariablesCommand = new JCommand("SaveVariablesCommand", OnSaveVariables);
            CancelSaveVariablesCommand = new JCommand("CancelSaveVariablesCommand", OnCancelSaveVariables);
             
            AddPatientViewModel = new AddPatientViewModel();
            AddPatientViewModel.OnPatientAdded += OnPatientAdded;

            EditPatientViewModel = new EditPatientViewModel();
            EditPatientViewModel.OnPatientEdited += OnPatientEdited;

            CalculateViewModel = new CalculateViewModel();
            CalculateViewModel.OnCalculate += OnCalculated;

            VariableExpressionViewModel = new VariableExpressionViewModel();
            VariableExpressionViewModel.OnVariablesEditCompleted += OnVariablesEditCompleted;

            MessageViewModel = new MessageViewModel();
            MessageViewModel.OnMessageClosed += OnMessageClosed;

            Patients = new ObservableCollection<Patient>();

            InitPatients();
        }

        private void OnSaveVariables(object obj)
        {
            Task.Run(() =>
            {
                try
                {
                    _dataHelper.SavePatientVariables(SelectPatient.Id, SelectPatient.Variables);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            });
        }

        private void OnCancelSaveVariables(object obj)
        {
            InitSelectPatientVariables();
        }

        private void InitPatients()
        {
            Task.Run(() =>
            {
                try
                {
                    var patients = _dataHelper.GetAllPatients();
                    if (patients == null)
                    {
                        return;
                    }
                    foreach (var patient in patients)
                    {
                        var id = patient.Id;
                        var name = patient.Name;
                        var birthday = patient.Birthday;
                        var weight = patient.Weight;

                        if (_dispatcher.CheckAccess())
                        {
                            Patients.Add(new Patient(id, name, birthday, weight));
                        }
                        else
                        {
                            _dispatcher.Invoke(() =>
                            {
                                Patients.Add(new Patient(id, name, birthday, weight));
                            });
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageViewModel.SetMessage($"{e.Message} \r\n {e.StackTrace}");
                    DialogViewModel = MessageViewModel;
                }
            });
        }
        private void InitSelectPatientVariables()
        {
            Task.Run(() =>
            {
                try
                {
                    if (SelectPatient != null)
                    {
                        SelectPatient.Variables = null;

                        var variables = _dataHelper.GetPatientVariables(SelectPatient.Id);
                        if (_dispatcher.CheckAccess())
                        {
                            if (variables != null && variables.Any())
                            {
                                SelectPatient.Variables = new ObservableCollection<Variable>(variables);
                            }
                            else
                            {
                                SelectPatient.GenerateDefaultVariables();
                            }
                        }
                        else
                        {
                            _dispatcher.Invoke(() =>
                            {
                                if (variables != null && variables.Any())
                                {
                                    SelectPatient.Variables = new ObservableCollection<Variable>(variables);
                                }
                                else
                                {
                                    SelectPatient.GenerateDefaultVariables();
                                }
                            });
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            });
        }

        private void OnEditVariableExpression(object obj)
        {
            if (SelectPatient.SelectedVariable == null)
            {
                MessageViewModel.SetMessage("请先选择一项数据");
                DialogViewModel = MessageViewModel;
                return;
            }

            VariableExpressionViewModel.SetPatient((Patient)SelectPatient.Clone());
            DialogViewModel = VariableExpressionViewModel;
        }
        private void OnVariablesEditCompleted(object sender, VariableEditEventArgs e)
        {
            if (!e.IsCancel)
            {
                //更新公式
                var noneExpression = e.ExpressionItems.FirstOrDefault(a => a.Name == "无公式");
                if (noneExpression != null && e.ExpressionItems.Count > 1)
                {
                    e.ExpressionItems.Remove(noneExpression);
                }

                SelectPatient.SelectedVariable.Formula.Expression = string.Join("",
                    e.ExpressionItems.Select(item => item.Name));
                SelectPatient.SelectedVariable.Formula.MetaExpression = string.Join(",",
                    e.ExpressionItems.Select(item => item.Name));

                SelectPatient.SelectedVariable.Formula.ExpressionItems.Clear();
                foreach (var item in e.ExpressionItems)
                {
                    SelectPatient.SelectedVariable.Formula.ExpressionItems.Add(new ExpressionItem(item.Name, item.Value));
                }
            }

            IsDialogOpen = false;
        }
        private void OnCalculated(object sender, PatientCalculateEventArgs e)
        {
            if (!e.IsCancel)
            {
                var calcId = Guid.NewGuid().ToString();

            }
            IsDialogOpen = false;
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
