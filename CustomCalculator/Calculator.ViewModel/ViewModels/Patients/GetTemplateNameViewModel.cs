using System;
using Calculator.Service.Services.App;
using Jg.wpf.core.Command;
using Jg.wpf.core.Notify;

namespace Calculator.ViewModel.ViewModels.Patients
{
    public class GetTemplateNameViewModel : ViewModelBase, IResult
    {
        public event EventHandler<object> OnResultChanged;

        public string Name { get; set; }
        public object Result { get; private set; }

        public JCommand ConfirmCommand { get; }
        public JCommand CancelCommand { get; }

        public GetTemplateNameViewModel()
        {
            ConfirmCommand = new JCommand("ConfirmCommand", OnConfirm);
            CancelCommand = new JCommand("CancelCommand", OnCancel);
        }

        private void OnConfirm(object obj)
        {
            Result = Name;
            OnResultChanged?.Invoke(this, Result);
        }

        private void OnCancel(object obj)
        {
            Result = "";
            OnResultChanged?.Invoke(this, Result);
        }

    }
}
