using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Calculator.Model.Models
{
    public class Formula
    {
        public List<Variable> Variables { get; }
        public Formula(List<Variable> variables)
        {
            Variables = variables;
        }

        public string Build()
        {
           return string.Join("", Variables.Select(v => v.Value));
        }
    }
}
