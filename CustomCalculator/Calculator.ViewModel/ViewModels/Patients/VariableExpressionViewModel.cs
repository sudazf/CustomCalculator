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

        public event EventHandler<VariableEditEventArgs> OnExpressionItemsEditCompleted;

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
        public ObservableCollection<JCommand> NumberCommands { get; }

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
            NumberCommands = new ObservableCollection<JCommand>
            {
                new JCommand("0", OnAddNumber0, null, "0"),
                new JCommand("1", OnAddNumber1, null, "1"),
                new JCommand("2", OnAddNumber2, null, "2"),
                new JCommand("3", OnAddNumber3, null, "3"),
                new JCommand("4", OnAddNumber4, null, "4"),
                new JCommand("5", OnAddNumber5, null, "5"),
                new JCommand("6", OnAddNumber6, null, "6"),
                new JCommand("7", OnAddNumber7, null, "7"),
                new JCommand("8", OnAddNumber8, null, "8"),
                new JCommand("9", OnAddNumber9, null, "9"),
                new JCommand(".", OnAddNumberDot, null, "."),
            };
        }


        public void SetPatient(Patient patient)
        {
            Patient = patient;
            VariableCommands.Clear();

            foreach (var variable in Patient.SelectedDay.Variables)
            {
                VariableCommands.Add(new JCommand(variable.Id, OnAddExpressionItemToVariable, null, variable.Name));
            }
        }

        private void OnSave(object obj)
        {
            OnExpressionItemsEditCompleted?.Invoke(this, new VariableEditEventArgs(false, Patient.SelectedDay.SelectedVariable.Formula.ExpressionItems.ToList()));
        }
        private void OnCancel(object obj)
        {
            OnExpressionItemsEditCompleted?.Invoke(this, new VariableEditEventArgs(true, Patient.SelectedDay.SelectedVariable.Formula.ExpressionItems.ToList()));
        }

        private void OnAddExpressionItemToVariable(object obj)
        {
            if (obj is string variableId)
            {
                var variable = Patient.SelectedDay.Variables.FirstOrDefault(v => v.Id == variableId);
                if (variable != null)
                {
                    Patient.SelectedDay.SelectedVariable.Formula.ExpressionItems.Insert(ExpressionItemIndex, new ExpressionItem(variable.Name, variable.Value));
                }
            }
        }

        private void OnAddVariableAddition(object obj)
        {
            Patient.SelectedDay.SelectedVariable.Formula.ExpressionItems.Insert(ExpressionItemIndex, new ExpressionItem("+", "+"));
        }
        private void OnAddVariableSubtraction(object obj)
        {
            Patient.SelectedDay.SelectedVariable.Formula.ExpressionItems.Insert(ExpressionItemIndex, new ExpressionItem("-", "-"));
        }
        private void OnAddVariableMultiplication(object obj)
        {
            Patient.SelectedDay.SelectedVariable.Formula.ExpressionItems.Insert(ExpressionItemIndex, new ExpressionItem("*", "*"));
        }
        private void OnAddVariableDivision(object obj)
        {
            Patient.SelectedDay.SelectedVariable.Formula.ExpressionItems.Insert(ExpressionItemIndex, new ExpressionItem("/", "/"));
        }
        private void OnAddVariableBracket_Left(object obj)
        {
            Patient.SelectedDay.SelectedVariable.Formula.ExpressionItems.Insert(ExpressionItemIndex, new ExpressionItem("(", "("));
        }
        private void OnAddVariableBracket_Right(object obj)
        {
            Patient.SelectedDay.SelectedVariable.Formula.ExpressionItems.Insert(ExpressionItemIndex, new ExpressionItem(")", ")"));
        }

        private void OnAddNumber0(object obj)
        {
            Patient.SelectedDay.SelectedVariable.Formula.ExpressionItems.Insert(ExpressionItemIndex, new ExpressionItem("0", "0"));
        }
        private void OnAddNumber1(object obj)
        {
            Patient.SelectedDay.SelectedVariable.Formula.ExpressionItems.Insert(ExpressionItemIndex, new ExpressionItem("1", "1"));
        }
        private void OnAddNumber2(object obj)
        {
            Patient.SelectedDay.SelectedVariable.Formula.ExpressionItems.Insert(ExpressionItemIndex, new ExpressionItem("2", "2"));
        }
        private void OnAddNumber3(object obj)
        {
            Patient.SelectedDay.SelectedVariable.Formula.ExpressionItems.Insert(ExpressionItemIndex, new ExpressionItem("3", "3"));
        }
        private void OnAddNumber4(object obj)
        {
            Patient.SelectedDay.SelectedVariable.Formula.ExpressionItems.Insert(ExpressionItemIndex, new ExpressionItem("4", "4"));
        }
        private void OnAddNumber5(object obj)
        {
            Patient.SelectedDay.SelectedVariable.Formula.ExpressionItems.Insert(ExpressionItemIndex, new ExpressionItem("5", "5"));
        }
        private void OnAddNumber6(object obj)
        {
            Patient.SelectedDay.SelectedVariable.Formula.ExpressionItems.Insert(ExpressionItemIndex, new ExpressionItem("6", "6"));
        }
        private void OnAddNumber7(object obj)
        {
            Patient.SelectedDay.SelectedVariable.Formula.ExpressionItems.Insert(ExpressionItemIndex, new ExpressionItem("7", "7"));
        }
        private void OnAddNumber8(object obj)
        {
            Patient.SelectedDay.SelectedVariable.Formula.ExpressionItems.Insert(ExpressionItemIndex, new ExpressionItem("8", "8"));
        }
        private void OnAddNumber9(object obj)
        {
            Patient.SelectedDay.SelectedVariable.Formula.ExpressionItems.Insert(ExpressionItemIndex, new ExpressionItem("9", "9"));
        }
        private void OnAddNumberDot(object obj)
        {
            Patient.SelectedDay.SelectedVariable.Formula.ExpressionItems.Insert(ExpressionItemIndex, new ExpressionItem(".", "."));
        }
    }
}
