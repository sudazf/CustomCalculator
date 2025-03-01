using System;
using Calculator.Model.Events;
using Jg.wpf.core.Notify;

namespace Calculator.Model.Models
{
    public class SimpleVariableTemplate : ViewModelBase
    {
        private string _name;
        private string _oldName;
        private bool _isChecked;

        public event EventHandler OnCheckedChanged;
        public event EventHandler<TemplateNameChangedEventArgs> OnNameChanged;


        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (value == _isChecked) return;
                _isChecked = value;
                OnCheckedChanged?.Invoke(this, EventArgs.Empty);
                RaisePropertyChanged(nameof(IsChecked));
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (value == _name) return;
                _oldName = _name;
                _name = value;
                RaisePropertyChanged(nameof(Name));
                OnNameChanged?.Invoke(this, new TemplateNameChangedEventArgs(_oldName, _name, this));
            }
        }

        public SimpleVariableTemplate(string name)
        {
            Name = name;
        }

        public void UpdateCheck(bool isCheckedAll)
        {
            _isChecked = isCheckedAll;

            RaisePropertyChanged(nameof(IsChecked));
        }

        public void RevertName()
        {
            _name = _oldName;
            RaisePropertyChanged(nameof(Name));
        }
    }
}
