using Jg.wpf.core.Notify;
using System.Collections.ObjectModel;
using System;
using Jg.wpf.core.Command;
using Jg.wpf.core.Extensions.Types.Animations;
using System.Linq;

namespace Calculator.Model.Models
{
    public class DailyInfo : ViewModelBase, ICloneable
    {
        private Variable _selectedVariable;
        private ObservableCollection<Variable> _variables;
        private string _day;
        private bool _isCheckedAll;

        public event EventHandler OnSelectedVariableChanged;

        public string Day
        {
            get => _day;
            set
            {
                if (value == _day) return;
                _day = value;
                RaisePropertyChanged(nameof(Day));
            }
        }
        public bool IsCheckedAll
        {
            get => _isCheckedAll;
            set
            {
                if (value == _isCheckedAll) return;

                foreach (var variable in Variables)
                {
                    variable.AutoUpdateCheckState(!_isCheckedAll);
                }

                _isCheckedAll = value;
                RaisePropertyChanged(nameof(IsCheckedAll));
            }
        }
        public ObservableCollection<Variable> Variables
        {
            get => _variables;
            set
            {
                if (Equals(value, _variables)) return;
                _variables = value;
                RaisePropertyChanged(nameof(Variables));
            }
        }
        public Variable SelectedVariable
        {
            get => _selectedVariable;
            set
            {
                if (Equals(value, _selectedVariable)) return;
                _selectedVariable = value;
                RaisePropertyChanged(nameof(SelectedVariable));

                OnSelectedVariableChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public JCommand VariablesDroppedCommand { get; }

        public DailyInfo()
        {
            VariablesDroppedCommand = new JCommand("VariablesDroppedCommand", OnVariablesDropped, a => true, "ItemDropped");
            _variables = new ObservableCollection<Variable>();
        }

        public void RaiseCheckedAll()
        {
            var selectedCount = Variables.Count(a => a.IsChecked);
            _isCheckedAll = selectedCount == Variables.Count;
            RaisePropertyChanged(nameof(IsCheckedAll));
        }
        public void UpdateSelect()
        {
            RaisePropertyChanged(nameof(Variables));
        }
        public object Clone()
        {
            var clone = new DailyInfo();
            clone.Day = _day;

            if (Variables != null)
            {
                var variables = new ObservableCollection<Variable>();
                foreach (var variable in Variables)
                {
                    variables.Add((Variable)variable.Clone());
                }
                clone.Variables = variables;
            }

            if (SelectedVariable != null)
            {
                clone.SelectedVariable = (Variable)SelectedVariable.Clone();
            }

            return clone;
        }

        private void OnVariablesDropped(object obj)
        {
            if (obj is IItemDroppedEventArgs args &&
                args.CurrentIndex >= 0 && args.PreviousIndex >= 0 &&
                args.CurrentIndex != args.PreviousIndex)
            {
                var p = args.PreviousIndex;
                var c = args.CurrentIndex;

                Variables.Move(p, c);
            }
        }
    }
}
