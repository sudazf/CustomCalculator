using Calculator.Service.Services.App;
using Jg.wpf.core.Command;
using Jg.wpf.core.Notify;
using System;
using System.Collections.Generic;

namespace Calculator.ViewModel.ViewModels.Patients
{
    public class SelectTemplateNameViewModel : ViewModelBase, IResult
    {
        public event EventHandler<object> OnResultChanged;

        public string SelectedName { get; set; }
        public object Result { get; private set; }

        public List<string> Names { get; set; }

        public JCommand ConfirmCommand { get; }
        public JCommand CancelCommand { get; }

        public SelectTemplateNameViewModel(List<string> names)
        {
            Names = names;

            ConfirmCommand = new JCommand("ConfirmCommand", OnConfirm);
            CancelCommand = new JCommand("CancelCommand", OnCancel);
        }

        private void OnConfirm(object obj)
        {
            Result = SelectedName;
            OnResultChanged?.Invoke(this, Result);
        }

        private void OnCancel(object obj)
        {
            Result = "";
            OnResultChanged?.Invoke(this, Result);
        }

    }
}
