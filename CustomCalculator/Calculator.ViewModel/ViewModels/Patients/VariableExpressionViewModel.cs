using System;
using System.Collections.ObjectModel;
using System.Linq;
using Calculator.Model.Events;
using Calculator.Model.Models;
using Jg.wpf.core.Command;
using Jg.wpf.core.Notify;

namespace Calculator.ViewModel.ViewModels.Patients
{
    public class VariableExpressionViewModel : ViewModelBase
    {
        private Variable _variable;
        private Patient _patient;

        public event EventHandler<VariableEditEventArgs> OnExpressionEdited;

        public Patient Patient
        {
            get => _patient;
            private set
            {
                if (Equals(value, _patient)) return;
                _patient = value;
                RaisePropertyChanged(nameof(Patient));
            }
        }

        public Variable Variable
        {
            get => _variable;
            set
            {
                if (Equals(value, _variable)) return;
                _variable = value;
                RaisePropertyChanged(nameof(Variable));
            }
        }

        public ObservableCollection<JCommand> VariableCommands { get; }
        public ObservableCollection<JCommand> MathCommands { get; }

        public JCommand SaveCommand { get; }
        public JCommand CancelCommand { get; }

        public VariableExpressionViewModel()
        {
            SaveCommand = new JCommand("SaveCommand", OnSave);
            CancelCommand = new JCommand("CancelCommand", OnCancel);

            VariableCommands = new ObservableCollection<JCommand>();
            MathCommands = new ObservableCollection<JCommand>();

            MathCommands.Add(new JCommand("+", OnAddVariableAddition, null, "+"));
            MathCommands.Add(new JCommand("-", OnAddVariableSubtraction, null, "-"));
            MathCommands.Add(new JCommand("*", OnAddVariableMultiplication, null, "*"));
            MathCommands.Add(new JCommand("/", OnAddVariableDivision, null, "/"));
            MathCommands.Add(new JCommand("(", OnAddVariableBracket_Left, null, "("));
            MathCommands.Add(new JCommand(")", OnAddVariableBracket_Right, null, ")"));
        }

        public void SetVariable(Variable variable)
        {
            Variable = variable;
        }

        public void SetPatient(Patient patient)
        {
            Patient = patient;
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
                    Patient.SelectedVariable.Formula.ExpressionItems.Add(new ExpressionItem(variable.Name, variable.Value));
                }
            }
        }

        private void OnAddVariableAddition(object obj)
        {
            Patient.SelectedVariable.Formula.ExpressionItems.Add(new ExpressionItem("+", "+"));
        }

        private void OnAddVariableSubtraction(object obj)
        {
            Patient.SelectedVariable.Formula.ExpressionItems.Add(new ExpressionItem("-", "-"));
        }

        private void OnAddVariableMultiplication(object obj)
        {
            Patient.SelectedVariable.Formula.ExpressionItems.Add(new ExpressionItem("*", "*"));
        }

        private void OnAddVariableDivision(object obj)
        {
            Patient.SelectedVariable.Formula.ExpressionItems.Add(new ExpressionItem("/", "/"));
        }

        private void OnAddVariableBracket_Left(object obj)
        {
            Patient.SelectedVariable.Formula.ExpressionItems.Add(new ExpressionItem("(", "("));
        }

        private void OnAddVariableBracket_Right(object obj)
        {
            Patient.SelectedVariable.Formula.ExpressionItems.Add(new ExpressionItem(")", ")"));
        }

        private void OnSave(object obj)
        {
            OnExpressionEdited?.Invoke(this, new VariableEditEventArgs(false, 
                string.Join("", Patient.SelectedVariable.Formula.ExpressionItems.Select(v=>v.Name)),
                    string.Join(",", Patient.SelectedVariable.Formula.ExpressionItems.Select(v => v.Name))));
        }

        private void OnCancel(object obj)
        {
            OnExpressionEdited?.Invoke(this, new VariableEditEventArgs(true, "",""));
        }
    }
}
