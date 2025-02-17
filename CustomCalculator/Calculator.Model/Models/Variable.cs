using System;
using System.Collections.Generic;

namespace Calculator.Model.Models
{
    public class Variable : ICloneable
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Value { get; set; }
        public string Unit { get; private set; }
        public string Min { get; set; }
        public string Max { get; set; }

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

            return clone;
        }
    }
}
