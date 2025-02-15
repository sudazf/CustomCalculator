using Calculator.Model.Events;
using Calculator.Model.Models;
using Jg.wpf.core.Command;
using Jg.wpf.core.Notify;
using System;

namespace Calculator.ViewModel.ViewModels.Patients
{
    public class EditPatientViewModel : ViewModelBase
    {
        private Patient _editPatient;
        public event EventHandler<PatientAddOrEditEventArgs> OnPatientEdited;

        public Patient EditPatient
        {
            get => _editPatient;
            private set
            {
                if (Equals(value, _editPatient)) return;
                _editPatient = value;
                RaisePropertyChanged(nameof(EditPatient));
            }
        }

        public JCommand SavePatientCommand { get; }
        public JCommand CancelCommand { get; }

        public EditPatientViewModel()
        {
            SavePatientCommand = new JCommand("SavePatientCommand", OnSavePatient);
            CancelCommand = new JCommand("CancelCommand", OnCancel);
        }

        public void SetPatient(Patient editPatient)
        {
            EditPatient = editPatient;
        }
        private void OnCancel(object obj)
        {
            OnPatientEdited?.Invoke(this, new PatientAddOrEditEventArgs(EditPatient, true));
        }

        private void OnSavePatient(object obj)
        {
            OnPatientEdited?.Invoke(this, new PatientAddOrEditEventArgs(EditPatient, false));
        }
    }
}
