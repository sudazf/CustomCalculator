using System;
using System.Collections.ObjectModel;
using System.Globalization;
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
        private double _height;
        private string _sd;
        private string _sex;
        private bool _isDirty;
        private string _bmi;

        public event EventHandler OnSelectedDailyVariableChanged;
        public event EventHandler OnSelectedDailyAllVariableChanged;
        public event EventHandler OnCommonInfoChanged;
        
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

        public string Sex
        {
            get => _sex;
            set
            {
                if (value == _sex) return;
                _sex = value;
                RaisePropertyChanged(nameof(Sex));
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

        public double Height
        {
            get => _height;
            set
            {
                if (value.Equals(_height)) return;
                _height = value;
                RaisePropertyChanged(nameof(Height));
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

        public string BMI
        {
            get => _bmi;
            set
            {
                if (value.Equals(_bmi)) return;
                _bmi = value;
                RaisePropertyChanged(nameof(BMI));
            }
        }

        public string SD
        {
            get => _sd;
            set
            {
                if (value == _sd) return;
                _sd = value;
                RaisePropertyChanged(nameof(SD));
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

                OnCommonInfoChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool IsDirty
        {
            get => _isDirty;
            set
            {
                if (value == _isDirty) return;
                _isDirty = value;
                RaisePropertyChanged(nameof(IsDirty));
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
                    _selectedDay.OnSelectedAllChanged -= OnSelectedAllVariableChanged;
                }
                _selectedDay = value;
                if (_selectedDay != null)
                {
                    _selectedDay.OnSelectedVariableChanged += OnSelectedVariableChanged;
                    _selectedDay.OnSelectedAllChanged += OnSelectedAllVariableChanged;
                }
                RaisePropertyChanged(nameof(SelectedDay));
            }
        }

        public Patient()
        {
        }
        public Patient(string id, string bedNumber, string name, DateTime birthday, double weight, double height, string sex, string sd, string diagnosis) : this()
        {
            Id = id;
            BedNumber = bedNumber;
            Birthday = birthday;
            Weight = weight;
            Height = height;
            Sex = sex;
            SD = sd;
            Name = name;
            Diagnosis = diagnosis;
        }
        public Patient(string id, string bedNumber, string name, DateTime birthday, double weight, double height, string sex, string sd, string diagnosis,
            ObservableCollection<DailyInfo> days) : this(id, bedNumber,name, birthday, weight, height, sex, sd, diagnosis)
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
            var property1 = new Variable(Guid.NewGuid().ToString(), false,false,"P1", "0", "", "", "", new Formula("无公式"));
            var property2 = new Variable(Guid.NewGuid().ToString(), false,false,"P2", "0", "", "", "", new Formula("无公式"));
            var property3 = new Variable(Guid.NewGuid().ToString(), false,false,"P3", "0", "", "", "", new Formula("无公式"));
            var property4 = new Variable(Guid.NewGuid().ToString(), false, false, "P4", "0", "", "", "", new Formula("无公式"));

            var dailyInfo = new DailyInfo()
            {
                Day = DateTime.Now.ToString("yyyy-MM-dd"),
                Variables =
                {
                    property1, property2, property3, property4,
                },
            };
            dailyInfo.IsDirty = true;
            Days = new ObservableCollection<DailyInfo>
            {
                dailyInfo
            };
        }
        public void UpdateSelect()
        {
            RaisePropertyChanged(nameof(SelectedDay));
            SelectedDay?.UpdateSelect();
        }
        public void CalcBMI()
        {
            var months = SdHelper.BirthdayToMonth(Birthday);
            if (months > 60)
            {
                var adjHeight = Height / 100; //转成米
                BMI = Math.Round(Weight / (adjHeight * adjHeight)).ToString(CultureInfo.InvariantCulture); //bmi
            }
            else
            {
                BMI = "";
            }
        }
        public object Clone()
        {
            var clone = new Patient();
            clone.Id = string.Copy(Id);
            clone.BedNumber = string.Copy(BedNumber);
            clone.Name = string.Copy(Name);
            clone.Birthday = Birthday;
            clone.Weight = Weight;
            clone.Sex = Sex;
            clone.Height = Height;

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
        private void OnSelectedAllVariableChanged(object sender, EventArgs e)
        {
            OnSelectedDailyAllVariableChanged?.Invoke(this, e);
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

        public void ResetDiagnosis(string diagnosis)
        {
            _diagnosis = diagnosis;
            RaisePropertyChanged(nameof(Diagnosis));
        }
    }
}
