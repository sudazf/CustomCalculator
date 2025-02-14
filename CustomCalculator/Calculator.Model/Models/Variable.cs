using System;

namespace Calculator.Model.Models
{
    public class Variable
    {
        public string Id { get;}
        public string Name { get;}
        public string Value { get; set; }
        public string Unit { get; }
        public string Min { get; set; }
        public string Max { get; set; }

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
    }
}
