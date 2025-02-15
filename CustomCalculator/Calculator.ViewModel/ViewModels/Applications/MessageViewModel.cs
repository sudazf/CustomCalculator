using Jg.wpf.core.Command;
using Jg.wpf.core.Notify;
using System;

namespace Calculator.ViewModel.ViewModels.Applications
{
    public class MessageViewModel : ViewModelBase
    {
        public event EventHandler OnMessageClosed;

        public string Message { get; set; }

        public JCommand CloseMessageCommand { get; }


        public MessageViewModel()
        {
            CloseMessageCommand = new JCommand("CloseMessageCommand", OnCloseMessage);
        }

        private void OnCloseMessage(object obj)
        {
            OnMessageClosed?.Invoke(this, EventArgs.Empty);
        }

        public void SetMessage(string message)
        {
            Message = message;
        }
    }
}
