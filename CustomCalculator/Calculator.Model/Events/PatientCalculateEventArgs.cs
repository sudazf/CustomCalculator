using Calculator.Model.Models;
using System.Collections.ObjectModel;

namespace Calculator.Model.Events
{
    public class PatientCalculateEventArgs
    {
        public string PatientId { get; }
        public Variable Property1 { get;}
        public Variable Property2 { get;}
        public Variable Property3 { get;}
        public Variable Property4 { get;}

        public ObservableCollection<Variable> Variables { get; }

        public bool IsCancel { get; }

        public PatientCalculateEventArgs(string patientId, Variable property1, Variable property2, Variable property3, Variable property4, ObservableCollection<Variable> variables, bool isCancel)
        {
            PatientId = patientId;
            Property1 = property1;
            Property2 = property2;
            Property3 = property3;
            Property4 = property4;
            Variables = variables;
            IsCancel = isCancel;
        }
    }
}
