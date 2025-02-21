using System;
using Calculator.Service.Services.App;
using Jg.wpf.core.Command;
using Jg.wpf.core.Notify;

namespace Calculator.ViewModel.ViewModels.Applications
{
    public class ConfirmViewModel : ViewModelBase, IResult
    {
        public event EventHandler<object> OnResultChanged;
        public string Message { get; }
        public object Result { get; private set; }

        public JCommand ConfirmCommand { get; }
        public JCommand CancelCommand { get; }

        public ConfirmViewModel(string message)
        {
            Message = message;

            ConfirmCommand = new JCommand("ConfirmCommand", OnConfirm);
            CancelCommand = new JCommand("CancelCommand", OnCancel);
        }

        private void OnConfirm(object obj)
        {
            Result = true;
            OnResultChanged?.Invoke(this, Result);
        }

        private void OnCancel(object obj)
        {
            Result = false;
            OnResultChanged?.Invoke(this, Result);
        }
    }
}
