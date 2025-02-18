using System;

namespace Calculator.Model.Models
{
    public class ExpressionItem : ICloneable
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public ExpressionItem()
        {
            
        }
        public ExpressionItem(string name, string value)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Value = value;
        }

        public object Clone()
        {
            var clone = new ExpressionItem();
            clone.Id = Id;
            clone.Name = Name;
            clone.Value = Value;

            return clone;
        }
    }
}
