using System;
using System.Collections.ObjectModel;
using Calculator.Model.Events;
using Jg.wpf.core.Notify;

namespace Calculator.Model.Models
{
    public class Variable : ViewModelBase, ICloneable
    {
        private Formula _formula;
        private string _name;
        private string _value;
        private string _unit;
        private string _min;
        private string _max;

        public event EventHandler<VariablePropertyChangedEventArgs> OnPropertyChanged;

        public ObservableCollection<Variable> Container { get; set; }

        public string Id { get; set; }

        public string Name
        {
            get => _name;
            set
            {
                if (value == _name) return;
                var oldValue = _name;
                _name = value;
                OnPropertyChanged?.Invoke(this, new VariablePropertyChangedEventArgs(this, "Name", oldValue, _name));
            }
        }

        public string Value
        {
            get => _value;
            set
            {
                if (value == _value) return;
                var oldValue = _value;
                _value = value;
                OnPropertyChanged?.Invoke(this, new VariablePropertyChangedEventArgs(this, "Value", oldValue, _value));
            }
        }

        public string Unit
        {
            get => _unit;
            set
            {
                if (value == _unit) return;
                var oldValue = _value;
                _unit = value;
                OnPropertyChanged?.Invoke(this, new VariablePropertyChangedEventArgs(this, "Unit", oldValue, _unit));
            }
        }

        public string Min
        {
            get => _min;
            set
            {
                if (value == _min) return;
                var oldValue = _value;
                _min = value;
                OnPropertyChanged?.Invoke(this, new VariablePropertyChangedEventArgs(this, "Min", oldValue, _min));
            }
        }

        public string Max
        {
            get => _max;
            set
            {
                if (value == _max) return;
                var oldValue = _value;
                _max = value;
                OnPropertyChanged?.Invoke(this, new VariablePropertyChangedEventArgs(this, "Max", oldValue,_max));
            }
        }

        public Formula Formula
        {
            get => _formula;
            set
            {
                if (Equals(value, _formula)) return;
                _formula = value;
                RaisePropertyChanged(nameof(Formula));
            }
        }

        public Variable()
        {
        }

        public Variable(string id, string name, string value, 
            string unit = "", string min = "", string max = "",
            Formula formula = null)
        {
            Id = id;
            Name = name;
            Value = value;
            Unit = unit;
            Min = min;
            Max = max;
            Formula = formula;
        }

        public void RevertName(string old)
        {
            _name = old;
            RaisePropertyChanged(nameof(Name));
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

            if (Formula != null)
            {
                clone.Formula = (Formula)Formula.Clone();
            }

            return clone;
        }
    }
}
