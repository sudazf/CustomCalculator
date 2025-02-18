using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Calculator.Model.Events;
using Calculator.Model.Models;
using Calculator.Service.Services.Parser;
using Jg.wpf.core.Command;
using Jg.wpf.core.Notify;
using Jg.wpf.core.Service;
using Newtonsoft.Json.Linq;

namespace Calculator.ViewModel.ViewModels.Patients
{
    public class CalculateViewModel : ViewModelBase
    {
        private readonly IParser _parser;
        private Patient _patient;
        private double _calcResult;

        public Patient Patient => _patient;

        public double CalcResult
        {
            get => _calcResult;
            set
            {
                if (value.Equals(_calcResult)) return;
                _calcResult = value;
                RaisePropertyChanged(nameof(CalcResult));
            }
        }

        public event EventHandler<PatientCalculateEventArgs> OnCalculate;

        public JCommand CalculateCommand { get; }
        public JCommand SaveCommand { get; }
        public JCommand CancelCommand { get; }



        public CalculateViewModel()
        {
            _parser = ServiceManager.GetService<IParser>();

            CalculateCommand = new JCommand("CalculateCommand", OnCalc);
            SaveCommand = new JCommand("SavePatientCommand", OnSave);
            CancelCommand = new JCommand("CancelCommand", OnCancel);
        }





        private void OnSave(object obj)
        {
            OnCalculate?.Invoke(this, new PatientCalculateEventArgs(Patient, false));
        }

        private void OnCancel(object obj)
        {
            OnCalculate?.Invoke(this, new PatientCalculateEventArgs(Patient, true));
        }


        private void OnCalc(object obj)
        {
            var formula = string.Join("", Patient.SelectedVariable.Formula.ExpressionItems.Select(f=>f.Value));
            CalcResult = _parser.Parse(formula);
        }
    }
}
