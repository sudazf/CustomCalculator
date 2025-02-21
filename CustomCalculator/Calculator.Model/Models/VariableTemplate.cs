using System;

namespace Calculator.Model.Models
{
    public class VariableTemplate
    {
        public string Id { get; }
        public string Name { get; }
        public Variable Variable { get; }

        public VariableTemplate(string name, Variable variable)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Variable = variable;
        }
    }
}
