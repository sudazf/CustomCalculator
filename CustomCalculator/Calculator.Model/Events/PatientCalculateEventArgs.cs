using Calculator.Model.Models;
using System.Collections.ObjectModel;

namespace Calculator.Model.Events
{
    public class PatientCalculateEventArgs
    {
        public Patient Patient { get; }

        public bool IsCancel { get; }

        public PatientCalculateEventArgs(Patient patient, bool isCancel)
        {
            Patient = patient;
            IsCancel = isCancel;
        }
    }
}
