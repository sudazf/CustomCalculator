using System;
using System.Collections.ObjectModel;
using System.Linq;
using Jg.wpf.core.Notify;

namespace Calculator.Model.Models
{
    public class Patient : ViewModelBase, ICloneable
    {
        private string _name;
        private DateTime _birthday;
        private double _weight;
        private ObservableCollection<DailyInfo> _days;
        private DailyInfo _selectedDay;

        public event EventHandler OnSelectedDailyVariableChanged; 

        public string Id { get; private set; }
        public string Name
        {
            get => _name;
            set
            {
                if (value == _name) return;
                _name = value;
                RaisePropertyChanged(nameof(Name));
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
        public double Weight
        {
            get => _weight;
            set
            {
                if (Math.Abs(value - _weight) < 0.0001) return;
                _weight = value;
                RaisePropertyChanged(nameof(Weight));
            }
        }
        public ObservableCollection<DailyInfo> Days
        {
            get => _days;
            set
            {
                if (Equals(value, _days)) return;
                _days = value;
                RaisePropertyChanged(nameof(Days));
            }
        }
        public DailyInfo SelectedDay
        {
            get => _selectedDay;
            set
            {
                if (Equals(value, _selectedDay)) return;
                if (_selectedDay != null)
                {
                    _selectedDay.OnSelectedVariableChanged -= OnSelectedVariableChanged;
                }
                _selectedDay = value;
                if (_selectedDay != null)
                {
                    _selectedDay.OnSelectedVariableChanged += OnSelectedVariableChanged;
                }
                RaisePropertyChanged(nameof(SelectedDay));
            }
        }

        public Patient()
        {
        }
        public Patient(string id, string name, DateTime birthday, double weight) : this()
        {
            Id = id;

            Birthday = birthday;
            Weight = weight;
            Name = name;
        }
        public Patient(string id, string name, DateTime birthday, double weight,
            ObservableCollection<DailyInfo> days) : this(id, name, birthday, weight)
        {
            if (days == null)
            {
                GenerateDefaultDays();
            }
            else
            {
                Days = days;
            }
        }

        public void GenerateDefaultDays()
        {
            //默认变量
            var property1 = new Variable(Guid.NewGuid().ToString(), false, "P1", "0", "", "", "", new Formula("无公式"));
            var property2 = new Variable(Guid.NewGuid().ToString(), false, "P2", "0", "", "", "", new Formula("无公式"));
            var property3 = new Variable(Guid.NewGuid().ToString(), false, "P3", "0", "", "", "", new Formula("无公式"));
            var property4 = new Variable(Guid.NewGuid().ToString(), false, "P4", "0", "", "", "", new Formula("无公式"));

            var dailyInfo = new DailyInfo()
            {
                Day = DateTime.Now.ToString("yyyy-MM-dd"),
                Variables =
                {
                    property1, property2, property3, property4,
                },
            };

            Days = new ObservableCollection<DailyInfo>
            {
                dailyInfo
            };
        }
        public void UpdateSelect()
        {
            SelectedDay = Days.First();
            SelectedDay.UpdateSelect();
        }
        public object Clone()
        {
            var clone = new Patient();
            clone.Id = string.Copy(Id);
            clone.Name = string.Copy(Name);
            clone.Birthday = Birthday;
            clone.Weight = Weight;

            if (Days != null)
            {
                var days = new ObservableCollection<DailyInfo>();
                foreach (var day in Days)
                {
                    days.Add((DailyInfo)day.Clone());
                }
                clone.Days = days;
            }

            if (SelectedDay != null)
            {
                clone.SelectedDay = (DailyInfo)SelectedDay.Clone();
            }

            return clone;
        }

        private void OnSelectedVariableChanged(object sender, EventArgs e)
        {
            OnSelectedDailyVariableChanged?.Invoke(this, e);
        }
    }
}
