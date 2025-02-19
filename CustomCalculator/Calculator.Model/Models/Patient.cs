using System;
using System.Collections.ObjectModel;
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
        private ObservableCollection<Variable> _variables;

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
                if (Math.Abs(value - _weight) < 0.0001) return;
                _weight = value;
                RaisePropertyChanged(nameof(Weight));
            }
        }

        //除常规信息以外的数据信息
        public ObservableCollection<Variable> Variables
        {
            get => _variables;
            set
            {
                if (Equals(value, _variables)) return;
                _variables = value;
                RaisePropertyChanged(nameof(Variables));
            }
        }

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

        public Patient(string id, string name, DateTime birthday, double weight)
        {
            Id = id;

            Birthday = birthday;
            Weight = weight;
            Name = name;

            AddVariableCommand = new JCommand("AddVariableCommand", OnAddVariable);
        }

        public Patient(string id, string name, DateTime birthday, double weight,
            ObservableCollection<Variable> variables) : this(id, name, birthday, weight)
        {
            if (variables == null)
            {
                GenerateDefaultVariables();
            }
            else
            {
                Variables = variables;
            }

            foreach (var patientVariable in Variables)
            {
                patientVariable.Container = Variables;
            }
        }

        public void GenerateDefaultVariables()
        {
            //默认变量
            var property1 = new Variable(Guid.NewGuid().ToString(), "P1", "0", "", "", "", new Formula("无公式"));
            var property2 = new Variable(Guid.NewGuid().ToString(), "P2", "0", "", "", "", new Formula("无公式"));
            var property3 = new Variable(Guid.NewGuid().ToString(), "P3", "0", "", "", "", new Formula("无公式"));
            var property4 = new Variable(Guid.NewGuid().ToString(), "P4", "0", "", "", "", new Formula("无公式"));

            Variables = new ObservableCollection<Variable>
            {
                property1,
                property2,
                property3,
                property4
            };
        }


        private void OnAddVariable(object obj)
        {
            //todo
        }

        public object Clone()
        {
            var clone = new Patient();
            clone.Id = string.Copy(Id);
            clone.Name = string.Copy(Name);
            clone.Birthday = Birthday;
            clone.Weight = Weight;

            if (Variables != null)
            {
                var variables = new ObservableCollection<Variable>();
                foreach (var variable in Variables)
                {
                    variables.Add((Variable)variable.Clone());
                }
                clone.Variables = variables;
            }

            if (SelectedVariable != null)
            {
                clone.SelectedVariable = (Variable)SelectedVariable.Clone();
            }

            return clone;
        }

    }
}
