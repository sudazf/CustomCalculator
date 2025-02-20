using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Calculator.Model.Events;
using Jg.wpf.core.Command;
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
        private bool _isChecked;

        public event EventHandler<VariablePropertyChangedEventArgs> OnPropertyChanged;

        public ObservableCollection<Variable> Container { get; set; }

        public string Id { get; set; }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (value == _isChecked) return;
                var oldValue = _isChecked;
                _isChecked = value;
                RaisePropertyChanged(nameof(IsChecked));
                OnPropertyChanged?.Invoke(this, new VariablePropertyChangedEventArgs(this, "IsChecked", oldValue.ToString(), _isChecked.ToString()));
            }
        }

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
                RaisePropertyChanged(nameof(Value));
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
                RaisePropertyChanged(nameof(Unit));
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
                RaisePropertyChanged(nameof(Min));
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
                RaisePropertyChanged(nameof(Max));
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


        public Variable(string id, bool isChecked, string name, string value, 
            string unit = "", string min = "", string max = "",
            Formula formula = null)
        {
            Id = id;
            Name = name;
            Value = value;
            Unit = unit;
            Min = min;
            Max = max;
            _isChecked = isChecked;
            _formula = formula;
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

        public void AutoUpdateCheckState(bool state)
        {
            _isChecked = state;
            RaisePropertyChanged(nameof(IsChecked));
        }
    }
}
