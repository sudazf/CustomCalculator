using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Calculator.Model.Events;
using Calculator.Model.Models;
using Calculator.Service.Services.Parser;
using Jg.wpf.core.Command;
using Jg.wpf.core.Notify;
using Jg.wpf.core.Service;
using Newtonsoft.Json.Linq;

namespace Calculator.ViewModel.ViewModels.Patients
{
    public class CalculateViewModel : ViewModelBase
    {
        private readonly IParser _parser;
        private Patient _patient;
        private double _calcResult;

        public Patient Patient => _patient;

        public double CalcResult
        {
            get => _calcResult;
            set
            {
                if (value.Equals(_calcResult)) return;
                _calcResult = value;
                RaisePropertyChanged(nameof(CalcResult));
            }
        }

        public event EventHandler<PatientCalculateEventArgs> OnCalculate;

        public JCommand CalculateCommand { get; }
        public JCommand SaveCommand { get; }
        public JCommand CancelCommand { get; }

        public List<JCommand> VariableCommands { get; }
        public List<JCommand> MathCommands { get; }

        public CalculateViewModel()
        {
            _parser = ServiceManager.GetService<IParser>();

            CalculateCommand = new JCommand("CalculateCommand", OnCalc);
            SaveCommand = new JCommand("SavePatientCommand", OnSave);
            CancelCommand = new JCommand("CancelCommand", OnCancel);

            VariableCommands = new List<JCommand>();
            MathCommands = new List<JCommand>();

            MathCommands.Add(new JCommand("+", OnAddVariableAddition, null, "+"));
            MathCommands.Add(new JCommand("-", OnAddVariableSubtraction, null, "-"));
            MathCommands.Add(new JCommand("*", OnAddVariableMultiplication, null, "*"));
            MathCommands.Add(new JCommand("/", OnAddVariableDivision, null, "/"));
            MathCommands.Add(new JCommand("(", OnAddVariableBracket_Left, null, "("));
            MathCommands.Add(new JCommand(")", OnAddVariableBracket_Right, null, ")"));
        }

        public void SetPatient(Patient patient)
        {
            _patient = patient;

            VariableCommands.Clear();
            foreach (var variable in Patient.Variables)
            {
                VariableCommands.Add(new JCommand(variable.Id, OnAddVariableToFormula, null, variable.Name));
            }
        }

        private void OnAddVariableToFormula(object obj)
        {
            if (obj is string variableId)
            {
                var variable = Patient.Variables.FirstOrDefault(v => v.Id == variableId);
                if (variable != null)
                {
                    Patient.FormulaVariables.Add(new Variable(variable.Name, variable.Value, variable.Unit, variable.Min, variable.Max));
                }
            }
        }


        private void OnAddVariableAddition(object obj)
        {
            Patient.FormulaVariables.Add(new Variable("+", "+", "", "", ""));
        }

        private void OnAddVariableSubtraction(object obj)
        {
            Patient.FormulaVariables.Add(new Variable("-", "-", "", "", ""));
        }

        private void OnAddVariableMultiplication(object obj)
        {
            Patient.FormulaVariables.Add(new Variable("*", "*", "", "", ""));
        }

        private void OnAddVariableDivision(object obj)
        {
            Patient.FormulaVariables.Add(new Variable("/", "/", "", "", ""));
        }

        private void OnAddVariableBracket_Left(object obj)
        {
            Patient.FormulaVariables.Add(new Variable("(", "(", "", "", ""));
        }

        private void OnAddVariableBracket_Right(object obj)
        {
            Patient.FormulaVariables.Add(new Variable(")", ")", "", "", ""));
        }

        private void OnSave(object obj)
        {
            OnCalculate?.Invoke(this, new PatientCalculateEventArgs(Patient, false));
        }

        private void OnCancel(object obj)
        {
            OnCalculate?.Invoke(this, new PatientCalculateEventArgs(Patient, true));
        }


        private void OnCalc(object obj)
        {
            var formula = string.Join("", Patient.FormulaVariables.Select(f=>f.Value));
            CalcResult = _parser.Parse(formula);
        }
    }
}
