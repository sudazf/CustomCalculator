using System;
using Calculator.Model.Events;
using Calculator.Model.Models;
using Jg.wpf.core.Command;
using Jg.wpf.core.Notify;

namespace Calculator.ViewModel.ViewModels.Patients
{
    public class AddPatientViewModel : ViewModelBase
    {
        private Patient _newPatient;

        public event EventHandler<PatientAddOrEditEventArgs> OnPatientAdded;

        public Patient NewPatient
        {
            get => _newPatient;
            private set
            {
                if (Equals(value, _newPatient)) return;
                _newPatient = value;
                RaisePropertyChanged(nameof(NewPatient));
            }
        }

        public JCommand SavePatientCommand { get; }
        public JCommand CancelCommand { get; }

        public AddPatientViewModel()
        {
            SavePatientCommand = new JCommand("SavePatientCommand", OnSavePatient);
            CancelCommand = new JCommand("CancelCommand", OnCancel);
        }

        public void SetPatient()
        {
            NewPatient = new Patient("", DateTime.Now, 0d);
        }

        private void OnCancel(object obj)
        {
            OnPatientAdded?.Invoke(this, new PatientAddOrEditEventArgs(NewPatient, true));
        }

        private void OnSavePatient(object obj)
        {
            OnPatientAdded?.Invoke(this, new PatientAddOrEditEventArgs(NewPatient, false));
        }
    }
}
