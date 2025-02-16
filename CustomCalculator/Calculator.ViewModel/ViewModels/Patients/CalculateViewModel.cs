using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Calculator.Model.Events;
using Calculator.Model.Models;
using Jg.wpf.core.Command;
using Jg.wpf.core.Notify;

namespace Calculator.ViewModel.ViewModels.Patients
{
    public class CalculateViewModel : ViewModelBase
    {
        private Patient _patient;
        private ObservableCollection<Variable> _settingVariables;

        public Patient Patient => _patient;

        public event EventHandler<PatientCalculateEventArgs> OnCalculate;

        public double Property1 { get; set; }
        public double Property2 { get; set; }
        public double Property3 { get; set; }
        public double Property4 { get; set; }

        public JCommand CalculateCommand { get; }
        public JCommand SaveCommand { get; }
        public JCommand CancelCommand { get; }

        public List<JCommand> VariableCommands { get; }

        public ObservableCollection<Variable> SettingVariables
        {
            get => _settingVariables;
            set
            {
                if (Equals(value, _settingVariables)) return;
                _settingVariables = value;
                RaisePropertyChanged(nameof(SettingVariables));
            }
        }

        public CalculateViewModel()
        {
            CalculateCommand = new JCommand("CalculateCommand", OnCalc);
            SaveCommand = new JCommand("SavePatientCommand", OnSave);
            CancelCommand = new JCommand("CancelCommand", OnCancel);

            SettingVariables = new ObservableCollection<Variable>();
            SettingVariables.CollectionChanged += SettingVariables_CollectionChanged;

            VariableCommands = new List<JCommand>();
            VariableCommands.Add(new JCommand("P1", OnAddVariableP1, null, "P1"));
            VariableCommands.Add(new JCommand("P2", OnAddVariableP2, null, "P2"));
            VariableCommands.Add(new JCommand("P3", OnAddVariableP3, null, "P3"));
            VariableCommands.Add(new JCommand("P4", OnAddVariableP4, null, "P4"));
            VariableCommands.Add(new JCommand("+", OnAddVariableAddition, null, "+"));
            VariableCommands.Add(new JCommand("-", OnAddVariableSubtraction, null, "-"));
            VariableCommands.Add(new JCommand("*", OnAddVariableMultiplication, null, "*"));
            VariableCommands.Add(new JCommand("/", OnAddVariableDivision, null, "/"));
            VariableCommands.Add(new JCommand("(", OnAddVariableBracket_Left, null, "("));
            VariableCommands.Add(new JCommand(")", OnAddVariableBracket_Right, null, ")"));
        }

        private void SettingVariables_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            
        }

        private void OnAddVariableAddition(object obj)
        {
            SettingVariables.Add(new Variable("+", "+", "", "", ""));
        }

        private void OnAddVariableSubtraction(object obj)
        {
            SettingVariables.Add(new Variable("-", "-", "", "", ""));
        }

        private void OnAddVariableMultiplication(object obj)
        {
            SettingVariables.Add(new Variable("*", "*", "", "", ""));
        }

        private void OnAddVariableDivision(object obj)
        {
            SettingVariables.Add(new Variable("/", "/", "", "", ""));
        }

        private void OnAddVariableBracket_Left(object obj)
        {
            SettingVariables.Add(new Variable("(", "(", "", "", ""));
        }

        private void OnAddVariableBracket_Right(object obj)
        {
            SettingVariables.Add(new Variable(")", ")", "", "", ""));
        }

        private void OnAddVariableP1(object obj)
        {
            SettingVariables.Add(new Variable("P1", "0", "A", "0", "100"));
        }

        private void OnAddVariableP2(object obj)
        {
            SettingVariables.Add(new Variable("P2", "0", "A", "0", "100"));
        }

        private void OnAddVariableP3(object obj)
        {
            SettingVariables.Add(new Variable("P3", "0", "A", "0", "100"));

        }

        private void OnAddVariableP4(object obj)
        {
            SettingVariables.Add(new Variable("P3", "0", "A", "0", "100"));
        }

        public void SetPatient(Patient patient)
        {
            _patient = patient;
        }

        private void OnSave(object obj)
        {
            OnCalculate?.Invoke(this, new PatientCalculateEventArgs(Property1, Property2, Property3, Property4, false));
        }

        private void OnCancel(object obj)
        {
            OnCalculate?.Invoke(this, new PatientCalculateEventArgs(Property1, Property2, Property3, Property4, true));
        }


        private void OnCalc(object obj)
        {
            //...
        }
    }
}
