using System;
using System.Collections.Generic;
using Calculator.Model.Models;
using Jg.wpf.core.Command;
using Jg.wpf.core.Notify;

namespace Calculator.ViewModel.ViewModels.Patients
{
    public class CorrectedAgeViewModel :ViewModelBase
    {
        private string _correctedAge;
        private DateTime _now = DateTime.Now;
        private DateTime _birthday = DateTime.Now;

        public DateTime Now
        {
            get => _now;
            set
            {
                if (value.Equals(_now)) return;
                _now = value;
                RaisePropertyChanged(nameof(Now));
            }
        }

        public DateTime Birthday
        {
            get => _birthday;
            set
            {
                if (value.Equals(_birthday)) return;
                _birthday = value;
                RaisePropertyChanged(nameof(Birthday));
            }
        }

        public int SelectedWeeks { get; set; }
        public int SelectedDays { get; set; }

        public List<int> WeekSource { get; }
        public List<int> DaySource { get; }
        public string CorrectedAge
        {
            get => _correctedAge;
            set
            {
                if (value == _correctedAge) return;
                _correctedAge = value;
                RaisePropertyChanged(nameof(CorrectedAge));
            }
        }

        public JCommand CalcCommand { get; }

        public CorrectedAgeViewModel()
        {
            WeekSource = new List<int>();
            DaySource = new List<int>();

            for (int i = 20; i < 37; i++)
            {
                WeekSource.Add(i);
            }

            for (int i = 0; i < 7; i++)
            {
                DaySource.Add(i);
            }

            SelectedWeeks = WeekSource[0];
            SelectedDays = DaySource[0];

            CalcCommand = new JCommand("CalcCommand", OnCalc);
        }

        public void OnCalc(object args)
        {
            var timeDifference = Now - Birthday;
            
            // 计算周和天
            int weeks = (timeDifference.Days + SelectedDays) / 7;
            int remainDays = (timeDifference.Days + SelectedDays) % 7;

            var totalWeeks = SelectedWeeks + weeks;

            if (totalWeeks >= 44)
            {
                totalWeeks = totalWeeks - 40;
                var totalMonths = totalWeeks / 4;
                var remainWeeks = totalWeeks % 4;
                var totalRemainDays = remainWeeks * 7 + remainDays;

                CorrectedAge = $"矫正年龄为：{totalMonths}月{totalRemainDays}天";
            }
            else
            {
                CorrectedAge = $"矫正年龄为：{totalWeeks}周{remainDays}天";
            }
        }
    }
}
