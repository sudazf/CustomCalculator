using Calculator.Model.Models;

namespace Calculator.Model.Events
{
    public class VariablePropertyChangedEventArgs
    {
        public Variable Variable { get; }
        public string PropertyName { get; }
        public string OldValue { get; }
        public string NewValue { get; }

        public VariablePropertyChangedEventArgs(Variable variable, string propertyName, string oldValue, string newValue)
        {
            Variable = variable;
            PropertyName = propertyName;
            OldValue = oldValue;
            NewValue = newValue;
        }
 
    }
}
