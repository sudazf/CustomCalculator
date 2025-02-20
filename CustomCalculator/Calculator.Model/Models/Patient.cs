using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
                RemoveVariableCommand.RaiseCanExecuteChanged();
            }
        }

        public JCommand AddVariableCommand { get; }
        public JCommand RemoveVariableCommand { get; }

        public Patient()
        {
            AddVariableCommand = new JCommand("AddVariableCommand", OnAddVariable);
            RemoveVariableCommand = new JCommand("RemoveVariableCommand", OnRemoveVariable, CanRemoveVariable);
        }

        public Patient(string id, string name, DateTime birthday, double weight) : this()
        {
            Id = id;

            Birthday = birthday;
            Weight = weight;
            Name = name;
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
            var property1 = new Variable(Guid.NewGuid().ToString(), false, "P1", "0", "", "", "", new Formula("无公式"));
            var property2 = new Variable(Guid.NewGuid().ToString(), false, "P2", "0", "", "", "", new Formula("无公式"));
            var property3 = new Variable(Guid.NewGuid().ToString(), false, "P3", "0", "", "", "", new Formula("无公式"));
            var property4 = new Variable(Guid.NewGuid().ToString(), false, "P4", "0", "", "", "", new Formula("无公式"));

            Variables = new ObservableCollection<Variable>
            {
                property1,
                property2,
                property3,
                property4
            };
        }

        private bool CanRemoveVariable(object arg)
        {
            return SelectedVariable != null;
        }

        private void OnAddVariable(object obj)
        {
            int suffix = 1;
            var newName = $"data{suffix}";
            while (Variables.FirstOrDefault(v=>v.Name == newName) != null)
            {
                newName = $"data{++suffix}";
            }

            var newVariable = new Variable(Guid.NewGuid().ToString(), false, newName, "0", "", "", "", new Formula("无公式"));
            Variables.Add(newVariable);
        }

        private void OnRemoveVariable(object obj)
        {
            var removeIds = new List<string>();
            foreach (var variable in Variables)
            {
                if (variable.IsChecked)
                {
                    removeIds.Add(variable.Id);
                }
            }

            foreach (var removeId in removeIds)
            {
                var removedVariable = Variables.FirstOrDefault(r => r.Id == removeId);
                if (removedVariable != null)
                {
                    Variables.Remove(removedVariable);
                }
            }
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
