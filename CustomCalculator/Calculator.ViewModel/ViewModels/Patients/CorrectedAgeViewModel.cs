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
            int days = (timeDifference.Days + SelectedDays) % 7;

            var adjust = SelectedWeeks + weeks;

            if (adjust >= 44)
            {
                adjust = adjust - 40;
                var calcMonths = adjust / 4;
                var calcWeeks = adjust % 4;
                var calcDays = calcWeeks * 7 + days + SelectedDays;

                CorrectedAge = $"矫正年龄为：{calcMonths}月{calcDays}天";
            }
            else
            {
                CorrectedAge = $"矫正年龄为：{adjust}周{days}天";
            }
        }
    }
}
