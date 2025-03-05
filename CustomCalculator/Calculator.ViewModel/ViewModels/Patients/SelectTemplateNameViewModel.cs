using Calculator.Service.Services.App;
using Jg.wpf.core.Command;
using Jg.wpf.core.Notify;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Calculator.ViewModel.ViewModels.Patients
{
    public class SelectTemplateNameViewModel : ViewModelBase, IResult
    {
        private string _selectedName;
        private List<string> _metaNames;
        private List<string> _names;
        private bool _showAllNames;
        public event EventHandler<object> OnResultChanged;

        public string SelectedName
        {
            get => _selectedName;
            set
            {
                if (value == _selectedName) return;
                _selectedName = value;
                if (string.IsNullOrEmpty(_selectedName))
                {
                    Names = _metaNames;
                    ShowAllNames = true;
                }
                else
                {
                    Names = _metaNames.Where(t => t.Contains(_selectedName)).ToList();
                }
                RaisePropertyChanged(nameof(SelectedName));
            }
        }

        public bool ShowAllNames
        {
            get => _showAllNames;
            set
            {
                if (value == _showAllNames) return;
                _showAllNames = value;
                RaisePropertyChanged(nameof(ShowAllNames));
            }
        }

        public object Result { get; private set; }

        public List<string> Names
        {
            get => _names;
            set
            {
                if (Equals(value, _names)) return;
                _names = value;
                RaisePropertyChanged(nameof(Names));
            }
        }

        public JCommand ConfirmCommand { get; }
        public JCommand CancelCommand { get; }
        public JCommand ShowAllTemplatesCommand { get; }
        

        public SelectTemplateNameViewModel(List<string> names)
        {
            _metaNames = names;
            Names = names;
            if (names.Count > 0)
            {
                _showAllNames = true;
            }
            ConfirmCommand = new JCommand("ConfirmCommand", OnConfirm);
            CancelCommand = new JCommand("CancelCommand", OnCancel);
            ShowAllTemplatesCommand = new JCommand("ShowAllTemplatesCommand", OnShowAllTemplates);
        }

        private void OnShowAllTemplates(object obj)
        {
            ShowAllNames = false;
            ShowAllNames = true;
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
