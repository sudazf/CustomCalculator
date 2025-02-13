using System;

namespace Calculator.Model.Models
{
    public class Variable
    {
        public string Id { get;}
        public string Name { get;}
        public string Value { get; set; }

        public Variable(string name, string defaultValue)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Value = defaultValue;
        }
    }
}
