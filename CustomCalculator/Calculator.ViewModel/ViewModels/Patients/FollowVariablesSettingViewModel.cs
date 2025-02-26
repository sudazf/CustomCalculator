using System;
using System.Collections.Generic;
using System.Linq;
using Calculator.Model.Events;
using Calculator.Model.Models;
using Calculator.Service.Services.App;
using Jg.wpf.core.Command;
using Jg.wpf.core.Notify;

namespace Calculator.ViewModel.ViewModels.Patients
{
    public class FollowVariablesSettingViewModel : ViewModelBase, IResult
    {
        private Variable _variable;
        private DailyInfo _day;

        public event EventHandler<VariableFollowsSettingEventArgs> OnFollowsSettingCompleted;

        public Variable Variable
        {
            get => _variable;
            set
            {
                if (Equals(value, _variable)) return;
                _variable = value;
                RaisePropertyChanged(nameof(Variable));
            }
        }

        public DailyInfo Day
        {
            get => _day;
            set
            {
                if (Equals(value, _day)) return;
                _day = value;
                RaisePropertyChanged(nameof(Day));
            }
        }

        public JCommand SaveFollowsCommand { get; }
        public JCommand CancelFollowsCommand { get; }

        public FollowVariablesSettingViewModel()
        {
            SaveFollowsCommand = new JCommand("SaveFollowsCommand", OnSave);
            CancelFollowsCommand = new JCommand("CancelFollowsCommand", OnCancel);
        }

        public void SetVariable(Variable variable, DailyInfo day)
        {
            Variable = variable;
            Day = day;

            foreach (var name in variable.FollowVariables)
            {
                var va = day.Variables.FirstOrDefault(v => v.Name == name);
                if (va != null)
                {
                    va.IsSelected = true;
                }
            }
        }

        private void OnSave(object obj)
        {
            var follows = Day.Variables.Where(v => v.IsSelected).Select(a=>a.Name).ToArray();
            Variable.FollowVariables = new List<string>(follows);
            OnFollowsSettingCompleted?.Invoke(this, new VariableFollowsSettingEventArgs(Variable, false));
            OnResultChanged?.Invoke(this, Result);
        }

        private void OnCancel(object obj)
        {
            OnFollowsSettingCompleted?.Invoke(this, new VariableFollowsSettingEventArgs(Variable, true));
            OnResultChanged?.Invoke(this, Result);
        }

        public event EventHandler<object> OnResultChanged;
        public object Result { get; }
    }
}
