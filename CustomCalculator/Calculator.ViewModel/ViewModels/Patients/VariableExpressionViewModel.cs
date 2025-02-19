using System;
using System.Collections.ObjectModel;
using System.Linq;
using Calculator.Model.Events;
using Calculator.Model.Models;
using Calculator.Service.Services.Database;
using Jg.wpf.core.Command;
using Jg.wpf.core.Notify;
using Jg.wpf.core.Service;

namespace Calculator.ViewModel.ViewModels.Patients
{
    public class VariableExpressionViewModel : ViewModelBase
    {
        private Patient _patient;
        private int _expressionItemIndex;

        public event EventHandler<VariableEditEventArgs> OnVariablesEditCompleted;

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
        public ObservableCollection<JCommand> VariableCommands { get; }
        public ObservableCollection<JCommand> MathCommands { get; }

        public int ExpressionItemIndex
        {
            get => _expressionItemIndex;
            set
            {
                if (value == _expressionItemIndex) return;
                _expressionItemIndex = value;
                Console.WriteLine($"ExpressionItemIndex: {ExpressionItemIndex}");
                RaisePropertyChanged(nameof(ExpressionItemIndex));
            }
        }

        public JCommand SaveCommand { get; }
        public JCommand CancelCommand { get; }

        public VariableExpressionViewModel()
        {
            ServiceManager.GetService<ISQLiteDataService>();

            SaveCommand = new JCommand("SaveCommand", OnSave);
            CancelCommand = new JCommand("CancelCommand", OnCancel);

            VariableCommands = new ObservableCollection<JCommand>();
            MathCommands = new ObservableCollection<JCommand>
            {
                new JCommand("+", OnAddVariableAddition, null, "+"),
                new JCommand("-", OnAddVariableSubtraction, null, "-"),
                new JCommand("*", OnAddVariableMultiplication, null, "*"),
                new JCommand("/", OnAddVariableDivision, null, "/"),
                new JCommand("(", OnAddVariableBracket_Left, null, "("),
                new JCommand(")", OnAddVariableBracket_Right, null, ")")
            };
        }

        public void SetPatient(Patient patient)
        {
            Patient = patient;
            VariableCommands.Clear();

            foreach (var variable in Patient.Variables)
            {
                VariableCommands.Add(new JCommand(variable.Id, OnAddExpressionItemToVariable, null, variable.Name));
            }
        }

        private void OnAddExpressionItemToVariable(object obj)
        {
            if (obj is string variableId)
            {
                var variable = Patient.Variables.FirstOrDefault(v => v.Id == variableId);
                if (variable != null)
                {
                    Patient.SelectedVariable.Formula.ExpressionItems.Insert(ExpressionItemIndex, new ExpressionItem(variable.Name, variable.Value));
                }
            }
        }

        private void OnAddVariableAddition(object obj)
        {
            Patient.SelectedVariable.Formula.ExpressionItems.Insert(ExpressionItemIndex, new ExpressionItem("+", "+"));
        }
        private void OnAddVariableSubtraction(object obj)
        {
            Patient.SelectedVariable.Formula.ExpressionItems.Insert(ExpressionItemIndex, new ExpressionItem("-", "-"));
        }
        private void OnAddVariableMultiplication(object obj)
        {
            Patient.SelectedVariable.Formula.ExpressionItems.Insert(ExpressionItemIndex, new ExpressionItem("*", "*"));
        }
        private void OnAddVariableDivision(object obj)
        {
            Patient.SelectedVariable.Formula.ExpressionItems.Insert(ExpressionItemIndex, new ExpressionItem("/", "/"));
        }
        private void OnAddVariableBracket_Left(object obj)
        {
            Patient.SelectedVariable.Formula.ExpressionItems.Insert(ExpressionItemIndex, new ExpressionItem("(", "("));
        }
        private void OnAddVariableBracket_Right(object obj)
        {
            Patient.SelectedVariable.Formula.ExpressionItems.Insert(ExpressionItemIndex, new ExpressionItem(")", ")"));
        }

        private void OnSave(object obj)
        {
            OnVariablesEditCompleted?.Invoke(this, new VariableEditEventArgs(false, Patient.SelectedVariable.Formula.ExpressionItems.ToList()));
        }

        private void OnCancel(object obj)
        {
            OnVariablesEditCompleted?.Invoke(this, new VariableEditEventArgs(true, Patient.SelectedVariable.Formula.ExpressionItems.ToList()));
        }
    }
}
