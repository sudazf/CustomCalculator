using System;
using Calculator.Model.Events;
using Calculator.Model.Models;
using Jg.wpf.core.Command;

namespace Calculator.ViewModel.ViewModels.Patients
{
    public class PatientVariablesSettingViewModel
    {
        public event EventHandler<ConfirmEventArgs> OnSettingCompleted;

        public Patient Patient { get; private set; }

        public JCommand SaveCommand { get; }
        public JCommand CancelCommand { get; }

        public PatientVariablesSettingViewModel()
        {
            SaveCommand = new JCommand("SaveCommand", OnSave);
            CancelCommand = new JCommand("CancelCommand", OnCancel);
        }

        public void SetPatient(Patient patient)
        {
            Patient = patient;
        }

        private void OnSave(object obj)
        {
            OnSettingCompleted?.Invoke(this, new ConfirmEventArgs(false));
        }

        private void OnCancel(object obj)
        {
            OnSettingCompleted?.Invoke(this, new ConfirmEventArgs(true));
        }
    }
}
