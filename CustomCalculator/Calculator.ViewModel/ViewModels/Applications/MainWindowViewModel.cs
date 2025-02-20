using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Calculator.Model.Events;
using Calculator.Model.Models;
using Calculator.Service.Services.Database;
using Calculator.Service.Services.Parser;
using Calculator.ViewModel.Helpers;
using Calculator.ViewModel.ViewModels.Patients;
using Jg.wpf.core.Command;
using Jg.wpf.core.Extensions.Types.Animations;
using Jg.wpf.core.Notify;
using Jg.wpf.core.Profilers;
using Jg.wpf.core.Service;
using Newtonsoft.Json.Linq;

namespace Calculator.ViewModel.ViewModels.Applications
{
    public class MainWindowViewModel : ViewModelBase
    {
        private object _dialogViewModel;
        private bool _isDialogOpen;
        private readonly ISQLiteDataService _dbService;
        private readonly IParser _parser;
        private readonly IDispatcher _dispatcher;
        private Patient _selectPatient;
        private readonly PatientDataHelper _dataHelper;
        private bool _isCheckedAll;

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
                RaisePropertyChanged(nameof(SelectPatient));
                RemovePatientCommand.RaiseCanExecuteChanged();
            }
        }

        public bool IsCheckedAll
        {
            get => _isCheckedAll;
            set
            {
                if (value == _isCheckedAll) return;

                foreach (var variable in SelectPatient.Variables)
                {
                    variable.AutoUpdateCheckState(!_isCheckedAll);
                }

                _isCheckedAll = value;
                RaisePropertyChanged(nameof(IsCheckedAll));
            }
        }

        public JCommand AddPatientCommand { get; }
        public JCommand EditPatientCommand { get; }
        public JCommand RemovePatientCommand { get; }
        public JCommand EditVariableExpressionCommand { get;  }

        public JCommand SaveVariablesCommand { get; }
        public JCommand CancelSaveVariablesCommand { get; }
        public JCommand CalculateCommand { get; }
        public JCommand CalcSingleCommand { get; }
        public JCommand VariablesDroppedCommand { get; }
        public JCommand AddVariableCommand { get; }
        public JCommand RemoveVariableCommand { get; }

        public AddPatientViewModel AddPatientViewModel { get; }
        public EditPatientViewModel EditPatientViewModel { get; }
        public CalculateViewModel CalculateViewModel { get; }
        public VariableExpressionViewModel VariableExpressionViewModel { get; }
        public MessageViewModel MessageViewModel { get; }
        
        public MainWindowViewModel()
        {
            _parser = ServiceManager.GetService<IParser>();
            _dbService = ServiceManager.GetService<ISQLiteDataService>();
            _dispatcher = ServiceManager.GetService<IDispatcher>();
            _dataHelper = new PatientDataHelper();

            AddPatientCommand = new JCommand("AddPatientCommand", OnAddPatient);
            EditPatientCommand = new JCommand("EditPatientCommand", OnEditPatient);
            RemovePatientCommand = new JCommand("RemovePatientCommand", OnRemovePatient, CanRemovePatient);
            EditVariableExpressionCommand = new JCommand("EditVariableExpressionCommand", OnEditVariableExpression);
            SaveVariablesCommand = new JCommand("SaveVariablesCommand", OnSaveVariables);
            CancelSaveVariablesCommand = new JCommand("CancelSaveVariablesCommand", OnCancelSaveVariables);
            AddVariableCommand = new JCommand("AddVariableCommand", OnAddVariable);
            RemoveVariableCommand = new JCommand("RemoveVariableCommand", OnRemoveVariable, CanRemoveVariable);

            CalculateCommand = new JCommand("CalculateCommand", OnCalc);
            CalcSingleCommand = new JCommand("", OnCalcSingle);
            VariablesDroppedCommand = new JCommand("VariablesDroppedCommand", OnVariablesDropped, a => true, "ItemDropped");
            AddPatientViewModel = new AddPatientViewModel();
            AddPatientViewModel.OnPatientAdded += OnPatientAdded;

            EditPatientViewModel = new EditPatientViewModel();
            EditPatientViewModel.OnPatientEdited += OnPatientEdited;

            CalculateViewModel = new CalculateViewModel();
            CalculateViewModel.OnCalculate += OnCalculated;

            VariableExpressionViewModel = new VariableExpressionViewModel();
            VariableExpressionViewModel.OnExpressionItemsEditCompleted += OnExpressionItemsEditCompleted;

            MessageViewModel = new MessageViewModel();
            MessageViewModel.OnMessageClosed += OnMessageClosed;

            Patients = new ObservableCollection<Patient>();
        }

        public void InitPatients()
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
                            var newPatient = new Patient(id, name, birthday, weight);
                            newPatient.OnSelectedVariableChanged += NewPatient_OnSelectedVariableChanged;
                            Patients.Add(newPatient);
                        }
                        else
                        {
                            _dispatcher.Invoke(() =>
                            {
                                var newPatient = new Patient(id, name, birthday, weight);
                                newPatient.OnSelectedVariableChanged += NewPatient_OnSelectedVariableChanged;
                                Patients.Add(newPatient);
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

        private void NewPatient_OnSelectedVariableChanged(object sender, EventArgs e)
        {
            RemoveVariableCommand.RaiseCanExecuteChanged();
        }

        public void InitSelectPatientVariables()
        {
            Task.Run(() =>
            {
                try
                {
                    if (SelectPatient != null)
                    {
                        if (SelectPatient.Variables != null)
                        {
                            foreach (var patientVariable in SelectPatient.Variables)
                            {
                                patientVariable.OnPropertyChanged -= PatientVariable_OnPropertyChanged;
                            }
                        }

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

                        if (SelectPatient.Variables != null)
                        {
                            foreach (var patientVariable in SelectPatient.Variables)
                            {
                                patientVariable.OnPropertyChanged += PatientVariable_OnPropertyChanged;
                            }
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

        private void OnCalcSingle(object obj)
        {
            try
            {
                var expression = string.Join("", SelectPatient.SelectedVariable.Formula.ExpressionItems.Select(a => a.Value));
                if (string.IsNullOrEmpty(expression))
                {
                    ShowMessage("无公式不支持计算");
                    return;
                }

                SelectPatient.SelectedVariable.Value = _parser.Parse(expression).ToString(CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ShowMessage($"计算出错：{e.Message}");
            }
        }
        private void OnCalc(object obj)
        {
            try
            {
                foreach (var variable in SelectPatient.Variables)
                {
                    if (!variable.IsChecked)
                    {
                        continue;
                    }
                    var expression = string.Join("", variable.Formula.ExpressionItems.Select(a => a.Value));
                    if (expression.Contains("无公式"))
                    {
                        continue;
                    }

                    variable.Value = _parser.Parse(expression).ToString(CultureInfo.InvariantCulture);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ShowMessage($"Calc failed: {e.Message}");
            }
        }


        private void OnSaveVariables(object obj)
        {
            Task.Run(() =>
            {
                try
                {
                    _dataHelper.SavePatientVariables(SelectPatient.Id, SelectPatient.Variables);
                    ShowMessage("保存成功");
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
        private void OnExpressionItemsEditCompleted(object sender, VariableEditEventArgs e)
        {
            if (!e.IsCancel)
            {
                if (e.ExpressionItems.Count > 0)
                {
                    //更新公式
                    var noneExpression = e.ExpressionItems.FirstOrDefault(a => a.Name == "无公式");
                    if (noneExpression != null && e.ExpressionItems.Count > 1)
                    {
                        e.ExpressionItems.Remove(noneExpression);
                    }

                    SelectPatient.SelectedVariable.Formula.ExpressionItems.Clear();
                    foreach (var item in e.ExpressionItems)
                    {
                        SelectPatient.SelectedVariable.Formula.ExpressionItems.Add(new ExpressionItem(item.Name, item.Value));
                    }

                    SelectPatient.SelectedVariable.Formula.Expression = string.Join("",
                        e.ExpressionItems.Select(item => item.Name));
                    SelectPatient.SelectedVariable.Formula.MetaExpression = string.Join(",",
                        e.ExpressionItems.Select(item => item.Name));
                }
                else
                {
                    SelectPatient.SelectedVariable.Formula.ExpressionItems.Clear();
                    SelectPatient.SelectedVariable.Formula.Expression = "无公式";
                    SelectPatient.SelectedVariable.Formula.MetaExpression = "无公式";
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

        private void OnAddPatient(object obj)
        {
            AddPatientViewModel.SetPatient();
            DialogViewModel = AddPatientViewModel;
        }
        private void OnPatientAdded(object sender, PatientAddOrEditEventArgs args)
        {
            if (!args.IsCancel)
            {
                args.Patient.OnSelectedVariableChanged += NewPatient_OnSelectedVariableChanged;
                Patients.Add(args.Patient);

                _dbService.AddPatient(args.Patient.Id, args.Patient.Name, args.Patient.Birthday, args.Patient.Weight);
            }

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

        private void OnRemovePatient(object obj)
        {
            try
            {
                if (SelectPatient != null)
                {
                    _dbService.DeletePatient(SelectPatient.Id);
                    SelectPatient.OnSelectedVariableChanged -= NewPatient_OnSelectedVariableChanged;

                    Patients.Remove(SelectPatient);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ShowMessage(e.Message);
            }
        }
        private bool CanRemovePatient(object arg)
        {
            return SelectPatient != null;
        }

        private bool CanRemoveVariable(object arg)
        {
            if (SelectPatient != null)
            {
                if (SelectPatient.SelectedVariable != null)
                {
                    return true;
                }
            }

            return false;
        }

        private void OnAddVariable(object obj)
        {
            int suffix = 1;
            var newName = $"data{suffix}";
            while (SelectPatient.Variables.FirstOrDefault(v => v.Name == newName) != null)
            {
                newName = $"data{++suffix}";
            }

            var newVariable = new Variable(Guid.NewGuid().ToString(), false, newName, "0", "", "", "", new Formula("无公式"));
            newVariable.OnPropertyChanged += PatientVariable_OnPropertyChanged;

            SelectPatient.Variables.Add(newVariable);
        }

        private void OnRemoveVariable(object obj)
        {
            var removeIds = new List<string>();
            foreach (var variable in SelectPatient.Variables)
            {
                if (variable.IsChecked)
                {
                    removeIds.Add(variable.Id);
                }
            }

            foreach (var removeId in removeIds)
            {
                var removedVariable = SelectPatient.Variables.FirstOrDefault(r => r.Id == removeId);
                if (removedVariable != null)
                {
                    removedVariable.OnPropertyChanged -= PatientVariable_OnPropertyChanged;
                    SelectPatient.Variables.Remove(removedVariable);
                }
            }
        }

        private void PatientVariable_OnPropertyChanged(object sender, VariablePropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsChecked":
                    var checkedCount = SelectPatient.Variables.Count(a => a.IsChecked);
                    _isCheckedAll = checkedCount == SelectPatient.Variables.Count;
                    RaisePropertyChanged(nameof(IsCheckedAll));
                    break;
                case "Name":
                    var matches = SelectPatient.Variables.Where(v => v.Name == e.NewValue);
                    if (matches.Count() > 1)
                    {
                        MessageViewModel.Message = $"已存在名为 {e.NewValue} 的数据";
                        DialogViewModel = MessageViewModel;
                        e.Variable.RevertName(e.OldValue);
                    }
                    else
                    {
                        //更新所有公式里的变量
                        foreach (var patientVariable in SelectPatient.Variables)
                        {
                            var formula = patientVariable.Formula;
                            foreach (var expressionItem in formula.ExpressionItems)
                            {
                                if (expressionItem.Name == e.OldValue)
                                {
                                    expressionItem.Name = e.NewValue;
                                }
                            }

                            formula.Expression = string.Join("", formula.ExpressionItems.Select(c => c.Name));
                            formula.MetaExpression = string.Join(",", formula.ExpressionItems.Select(c => c.Name));

                            if (string.IsNullOrEmpty(formula.Expression))
                            {
                                formula.Expression = "无公式";
                                formula.MetaExpression = "无公式";
                            }
                        }
                    }
                    break;
                case "Value":
                    //更新所有公式里的变量
                    foreach (var patientVariable in SelectPatient.Variables)
                    {
                        var formula = patientVariable.Formula;
                        foreach (var expressionItem in formula.ExpressionItems)
                        {
                            if (expressionItem.Name == e.Variable.Name)
                            {
                                expressionItem.Value = e.NewValue;
                            }
                        }
                    }
                    break;
                case "Min":
                    break;
                case "Max":
                    break;
                case "Unit":
                    break;
            }
        }

        private void OnVariablesDropped(object obj)
        {
            if (obj is IItemDroppedEventArgs args &&
                args.CurrentIndex >= 0 && args.PreviousIndex >= 0 &&
                args.CurrentIndex != args.PreviousIndex)
            {
                var p = args.PreviousIndex;
                var c = args.CurrentIndex;

                SelectPatient.Variables.Move(p, c);
            }
        }

        private void ShowMessage(string message)
        {
            MessageViewModel.SetMessage(message);
            DialogViewModel = MessageViewModel;
        }
        private void OnMessageClosed(object sender, EventArgs e)
        {
            IsDialogOpen = false;
        }
    }
}
