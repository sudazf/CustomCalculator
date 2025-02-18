using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Jg.wpf.core.Command;
using Jg.wpf.core.Notify;

namespace Calculator.Model.Models
{
    public class Variable : ViewModelBase, ICloneable
    {
        private ObservableCollection<Formula> _formulas;
        private Formula _selectedFormula;
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Value { get; set; }
        public string Unit { get; private set; }
        public string Min { get; set; }
        public string Max { get; set; }

        public ObservableCollection<Formula> Formulas
        {
            get => _formulas;
            set
            {
                if (Equals(value, _formulas)) return;
                _formulas = value;
                RaisePropertyChanged(nameof(Formulas));
            }
        }

        public Formula SelectedFormula
        {
            get => _selectedFormula;
            set
            {
                if (Equals(value, _selectedFormula)) return;
                _selectedFormula = value;
                RaisePropertyChanged(nameof(SelectedFormula));
            }
        }

        public JCommand AddFormulaCommand { get; }

        public Variable()
        {
        }

        public Variable(string name, string defaultValue, 
            string unit = "", string min = "", string max = "")
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Value = defaultValue;
            Unit = unit;
            Min = min;
            Max = max;

            Formulas = new ObservableCollection<Formula>();

            AddFormulaCommand = new JCommand("AddFormulaCommand", OnAddFormula);
        }

        public object Clone()
        {
            var clone = new Variable();
            clone.Id = string.Copy(Id);
            clone.Name = string.Copy(Name);
            clone.Value = string.Copy(Value);
            clone.Unit = string.Copy(Unit);
            clone.Min = string.Copy(Min);
            clone.Max = string.Copy(Max);

            var formulas = new ObservableCollection<Formula>();
            foreach (var formula in Formulas)
            {
                formulas.Add((Formula)formula.Clone());
            }
            clone.Formulas = formulas;

            return clone;
        }

        private void OnAddFormula(object obj)
        {
            Formulas.Add(new Formula("P1,+,P2"));
        }
    }
}
