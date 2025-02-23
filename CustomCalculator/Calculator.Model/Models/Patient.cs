using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
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
        private string _age;
        private string _bedNumber;
        private string _diagnosis;

        public event EventHandler OnSelectedDailyVariableChanged; 

        public string Id { get; private set; }

        public string BedNumber
        {
            get => _bedNumber;
            set
            {
                if (value == _bedNumber) return;
                _bedNumber = value;
                RaisePropertyChanged(nameof(BedNumber));
            }
        }

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
                Age = BirthdayToAge(_birthday);
                RaisePropertyChanged(nameof(Birthday));
            }
        }

        public string Age
        {
            get => _age;
            set
            {
                if (value == _age) return;
                _age = value;
                RaisePropertyChanged(nameof(Age));
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

        public string Diagnosis
        {
            get => _diagnosis;
            set
            {
                if (value == _diagnosis) return;
                _diagnosis = value;
                RaisePropertyChanged(nameof(Diagnosis));
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
            }
        }

        public Patient()
        {
        }
        public Patient(string id, string bedNumber, string name, DateTime birthday, double weight, string diagnosis) : this()
        {
            Id = id;
            BedNumber = bedNumber;
            Birthday = birthday;
            Weight = weight;
            Name = name;
            Diagnosis = diagnosis;
        }
        public Patient(string id, string bedNumber, string name, DateTime birthday, double weight, string diagnosis,
            ObservableCollection<DailyInfo> days) : this(id, bedNumber,name, birthday, weight, diagnosis)
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
            RaisePropertyChanged(nameof(SelectedDay));
            SelectedDay.UpdateSelect();
        }
        public object Clone()
        {
            var clone = new Patient();
            clone.Id = string.Copy(Id);
            clone.BedNumber = string.Copy(BedNumber);
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

        private string BirthdayToAge(DateTime birthday)
        {
            var currentDate = DateTime.Now;

            var years = currentDate.Year - birthday.Year;
            var months = currentDate.Month - birthday.Month;

            if (months < 0)
            {
                years--;
                months += 12;
            }

            if (months == 0)
            {
                return $"{years}年";
            }

            return $"{years}年{months}个月";
        }
    }
}
