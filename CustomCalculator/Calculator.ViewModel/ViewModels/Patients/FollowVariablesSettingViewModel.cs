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
        private string _errorMessage;

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

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                if (value == _errorMessage) return;
                _errorMessage = value;
                RaisePropertyChanged(nameof(ErrorMessage));
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
            foreach (var follow in follows)
            {
                var variable = Day.Variables.FirstOrDefault(a => a.Name == follow);
                if (variable != null)
                {
                    if (variable.Name == Variable.Name)
                    {
                        ErrorMessage = $"不能和自身联动，请取消选择 {Variable.Name}";
                        return;
                    }
                    else
                    {
                        if (variable.FollowVariables.Contains(Variable.Name))
                        {
                            ErrorMessage = $"设置错误：{variable.Name} 和 {Variable.Name} 将产生无线循环联动，请取消选择 {variable.Name}";
                            return;
                        }
                    }
                }
            }

            ErrorMessage = string.Empty;
            Variable.FollowVariables = new List<string>(follows);
            OnFollowsSettingCompleted?.Invoke(this, new VariableFollowsSettingEventArgs(Variable, false));
            OnResultChanged?.Invoke(this, Result);
        }

        private void OnCancel(object obj)
        {
            ErrorMessage = string.Empty;
            OnFollowsSettingCompleted?.Invoke(this, new VariableFollowsSettingEventArgs(Variable, true));
            OnResultChanged?.Invoke(this, Result);
        }

        public event EventHandler<object> OnResultChanged;
        public object Result { get; }
    }
}
