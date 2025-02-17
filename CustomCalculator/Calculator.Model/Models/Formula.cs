using System;
using System.Collections.Generic;
using System.Linq;

namespace Calculator.Model.Models
{
    public class Formula : ICloneable
    {
        public string Id { get; set; }

        public List<Variable> Variables { get; private set; }

        public Formula()
        {
            
        }

        public Formula(List<Variable> variables)
        {
            Id = Guid.NewGuid().ToString();
            Variables = variables;
        }

        public string Build()
        {
           return string.Join("", Variables.Select(v => v.Value));
        }

        public object Clone()
        {
            var clone = new Formula();
            clone.Id = string.Copy(Id);

            if (Variables != null)
            {
                var variables = new List<Variable>();
                foreach (var v in Variables)
                {
                    variables.Add((Variable)v.Clone());
                }
                clone.Variables = variables;
            }

            return clone;
        }
    }
}
