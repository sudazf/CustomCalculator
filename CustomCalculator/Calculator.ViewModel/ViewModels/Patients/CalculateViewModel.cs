using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Calculator.Model.Events;
using Calculator.Model.Models;
using Jg.wpf.core.Command;
using Jg.wpf.core.Notify;
using Newtonsoft.Json.Linq;

namespace Calculator.ViewModel.ViewModels.Patients
{
    public class CalculateViewModel : ViewModelBase
    {
        private Patient _patient;
        private ObservableCollection<Variable> _settingVariables;

        public Patient Patient => _patient;

        public event EventHandler<PatientCalculateEventArgs> OnCalculate;

        public Variable Property1 { get; set; }
        public Variable Property2 { get; set; }
        public Variable Property3 { get; set; }
        public Variable Property4 { get; set; }

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

            Property1 = new Variable("P1", "0", "", "", "");
            Property2 = new Variable("P2", "0", "", "", "");
            Property3 = new Variable("P3", "0", "", "", "");
            Property4 = new Variable("P4", "0", "", "", "");

            SettingVariables = new ObservableCollection<Variable>();
            SettingVariables.CollectionChanged += SettingVariables_CollectionChanged;

            VariableCommands = new List<JCommand>();
            VariableCommands.Add(new JCommand(Property1.Id, OnAddVariableP1, null, Property1.Name));
            VariableCommands.Add(new JCommand(Property2.Id, OnAddVariableP2, null, Property2.Name));
            VariableCommands.Add(new JCommand(Property3.Id, OnAddVariableP3, null, Property3.Name));
            VariableCommands.Add(new JCommand(Property4.Id, OnAddVariableP4, null, Property4.Name));
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
            SettingVariables.Add(new Variable(Property1.Name, Property1.Value, Property1.Unit, Property1.Min, Property1.Max));
        }

        private void OnAddVariableP2(object obj)
        {
            SettingVariables.Add(new Variable(Property2.Name, Property2.Value, Property2.Unit, Property2.Min, Property2.Max));
        }

        private void OnAddVariableP3(object obj)
        {
            SettingVariables.Add(new Variable(Property3.Name, Property3.Value, Property3.Unit, Property3.Min, Property3.Max));
        }

        private void OnAddVariableP4(object obj)
        {
            SettingVariables.Add(new Variable(Property4.Name, Property4.Value, Property4.Unit, Property4.Min, Property4.Max));
        }

        public void SetPatient(Patient patient)
        {
            _patient = patient;
        }

        private void OnSave(object obj)
        {
            OnCalculate?.Invoke(this, new PatientCalculateEventArgs(Patient.Id, Property1, Property2, Property3, Property4, SettingVariables, false));
        }

        private void OnCancel(object obj)
        {
            OnCalculate?.Invoke(this, new PatientCalculateEventArgs(Patient.Id, Property1, Property2, Property3, Property4, SettingVariables, true));
        }


        private void OnCalc(object obj)
        {
            //...
        }
    }
}
