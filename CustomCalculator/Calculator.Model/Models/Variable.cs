using System;
using Jg.wpf.core.Notify;

namespace Calculator.Model.Models
{
    public class Variable : ViewModelBase, ICloneable
    {
        private Formula _formula;
        public string Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
        public string Min { get; set; }
        public string Max { get; set; }

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
