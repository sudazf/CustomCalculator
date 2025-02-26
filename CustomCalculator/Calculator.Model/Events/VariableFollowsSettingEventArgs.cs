using Calculator.Model.Models;

namespace Calculator.Model.Events
{
    public class VariableFollowsSettingEventArgs
    {
        public Variable Variable { get; }

        public bool IsCancel { get;}

        public VariableFollowsSettingEventArgs(Variable variable, bool isCancel)
        {
            Variable = variable;
            IsCancel = isCancel;
        }
    }
}
