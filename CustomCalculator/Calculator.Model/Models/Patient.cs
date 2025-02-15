using System;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using Jg.wpf.core.Command;
using Jg.wpf.core.Notify;

namespace Calculator.Model.Models
{
    public class Patient : ViewModelBase, ICloneable
    {
        private string _name;
        private DateTime _birthday;
        private double _weight;
        public string Id { get; private set; }

        public string Name
        {
            get => _name;
            set
            {
                if (value == _name) return;
                _name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }

        public DateTime Birthday
        {
            get => _birthday;
            set
            {
                if (value.Equals(_birthday)) return;
                _birthday = value;
                RaisePropertyChanged(nameof(Birthday));
            }
        }

        public double Weight
        {
            get => _weight;
            set
            {
                if (value == _weight) return;
                _weight = value;
                RaisePropertyChanged(nameof(Weight));
            }
        }

        public ObservableCollection<Variable> Variables { get; private set; }
        public ObservableCollection<Formula> Formulas { get; private set; }

        public JCommand AddVariableCommand { get; }
        public JCommand AddFormulaCommand { get; }

        public Patient()
        {
            
        }

        public Patient(string name, DateTime birthday, double weight)
        {
            Id = Guid.NewGuid().ToString();

            Birthday = birthday;
            Weight = weight;
            Name = name;

            Variables = new ObservableCollection<Variable>();
            Formulas = new ObservableCollection<Formula>();

            AddVariableCommand = new JCommand("AddVariableCommand", OnAddVariable);
            AddFormulaCommand = new JCommand("AddFormulaCommand", OnAddFormula);
        }

        private void OnAddVariable(object obj)
        {

        }

        private void OnAddFormula(object obj)
        {
            Formulas.Add(new Formula(Variables.ToList()));
        }

        public void Test()
        {
            
        }

        public string Build()
        {
            return Formulas[0].Build();
        }

        public object Clone()
        {
            var clone = new Patient();
            clone.Id = string.Copy(Id);
            clone.Name = string.Copy(Name);
            clone.Birthday = Birthday;
            clone.Weight = Weight;

            var formulas = new ObservableCollection<Formula>();
            var variables = new ObservableCollection<Variable>();

            foreach (var formula in Formulas)
            {
                formulas.Add((Formula)formula.Clone());
            }
            foreach (var variable in Variables)
            {
                variables.Add((Variable)variable.Clone());
            }
            clone.Formulas = formulas;
            clone.Variables = variables;

            return clone;
        }
    }
}
