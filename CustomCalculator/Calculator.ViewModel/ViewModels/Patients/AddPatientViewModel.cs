using System;
using Calculator.Model.Models;
using Jg.wpf.core.Command;
using Jg.wpf.core.Notify;

namespace Calculator.ViewModel.ViewModels.Patients
{
    public class AddPatientViewModel : ViewModelBase
    {
        private bool _isAddingPatient;

        public event EventHandler<Patient> OnAddedPatient; 
        public bool IsAddingPatient
        {
            get => _isAddingPatient;
            set
            {
                if (value == _isAddingPatient) return;
                _isAddingPatient = value;
                RaisePropertyChanged(nameof(IsAddingPatient));
            }
        }

        public Patient NewPatient { get; }

        public JCommand AddPatientCommand { get; }
        public JCommand SavePatientCommand { get; }
        public JCommand CloseCommand { get; }

        public AddPatientViewModel()
        {
            NewPatient = new Patient("", DateTime.Now, "0");

            AddPatientCommand = new JCommand("AddPatientCommand", OnAddPatient);
            SavePatientCommand = new JCommand("SavePatientCommand", OnSavePatient);
            CloseCommand = new JCommand("CloseCommand", OnClose);
        }

        private void OnAddPatient(object obj)
        {
            IsAddingPatient = true;
        }

        private void OnClose(object obj)
        {
            IsAddingPatient = false;
        }

        private void OnSavePatient(object obj)
        {
            OnAddedPatient?.Invoke(this, NewPatient);

            IsAddingPatient = false;
        }
    }
}
