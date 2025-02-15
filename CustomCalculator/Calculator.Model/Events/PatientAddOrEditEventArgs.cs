using Calculator.Model.Models;

namespace Calculator.Model.Events
{
    public class PatientAddOrEditEventArgs
    {
        public Patient Patient { get; }

        public bool IsCancel { get; }

        public PatientAddOrEditEventArgs(Patient patient, bool isCancel)
        {
            Patient = patient;
            IsCancel = isCancel;
        }
    }
}
