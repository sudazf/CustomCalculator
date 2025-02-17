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
        private Variable _selectedVariable;
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

        public ObservableCollection<Variable> Variables { get; set; }
        public ObservableCollection<Variable> FormulaVariables { get; set; }

        public Variable SelectedVariable
        {
            get => _selectedVariable;
            set
            {
                if (Equals(value, _selectedVariable)) return;
                _selectedVariable = value;
                RaisePropertyChanged(nameof(SelectedVariable));
            }
        }

        public JCommand AddVariableCommand { get; }

        public Patient()
        {
            
        }

        public Patient(string name, DateTime birthday, double weight)
        {
            Id = Guid.NewGuid().ToString();

            Birthday = birthday;
            Weight = weight;
            Name = name;

            var property1 = new Variable("P1", "0", "", "", "");
            var property2 = new Variable("P2", "0", "", "", "");
            var property3 = new Variable("P3", "0", "", "", "");
            var property4 = new Variable("P4", "0", "", "", "");

            Variables = new ObservableCollection<Variable>
            {
                property1,
                property2,
                property3,
                property4
            };

            FormulaVariables = new ObservableCollection<Variable>();

            AddVariableCommand = new JCommand("AddVariableCommand", OnAddVariable);
        }

        private void OnAddVariable(object obj)
        {
            //todo
        }


        public void Test()
        {
            
        }

        public string Build()
        {
            return "";
        }

        public object Clone()
        {
            var clone = new Patient();
            clone.Id = string.Copy(Id);
            clone.Name = string.Copy(Name);
            clone.Birthday = Birthday;
            clone.Weight = Weight;

            var variables = new ObservableCollection<Variable>();
            foreach (var variable in Variables)
            {
                variables.Add((Variable)variable.Clone());
            }
            clone.Variables = variables;

            var fVariables = new ObservableCollection<Variable>();
            foreach (var variable in FormulaVariables)
            {
                fVariables.Add((Variable)variable.Clone());
            }
            clone.FormulaVariables = fVariables;

            return clone;
        }
    }
}
